using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Mapbox.Unity.Location;
using System;
using TMPro;
using System.Collections.Generic;
using System.Globalization;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public FirebaseUser user;

    public string userId;

    public DatabaseReference dbReference;
    public PlayerStatsManager statsManager;
    public GeneralInventory inventory;

    //public DeviceLocationProviderAndroidNative locationProvider;

    //private double sessionTotalTraveledDistance;
    //public double SessionTotalTraveledDistance { get { return sessionTotalTraveledDistance; } set { sessionTotalTraveledDistance = value; } }

    [SerializeField]
    private User loadedUser;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        user = AuthManager.instance.GetFirebaseUser();
        userId = AuthManager.instance.GetFirebaseUserId();
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateOrUpdateUser() {

        List<string> tempQuickItems = inventory.GetWeaponItemsAsStrings();
        List<string> tempWeaponItems = inventory.GetWeaponItemsAsStrings();
        List<string> tempLoggedDays = loadedUser.loggedDays;


        // Gather items from both combat and general inventory
        tempQuickItems.AddRange(CombatInventory.instance.GetQuickItemsAsStrings());
        tempWeaponItems.Add(CombatInventory.instance.GetEquipedWeaponAsString());
        tempLoggedDays.Add(DateTime.Today.ToString("yyyy-MM-dd"));


        User newUser = new User {
            userId = this.user.UserId,
            weaponItems = tempWeaponItems,
            quickItems = tempQuickItems,
            loggedDays = tempLoggedDays,
            exp = statsManager.EarnedExperience,
            totalTraveledDistance = loadedUser.totalTraveledDistance + TraveledDistanceTracker.instance.CurrentTraveledDistance,
            totalDaysLogged = (IsCurrentDay(loadedUser.lastLoggedDay)) ? loadedUser.totalDaysLogged : loadedUser.totalDaysLogged + 1,
            lastLoggedDay = DateTime.Today.ToString("yyyy-MM-dd"),
            stats = new Stats {
                vitality = statsManager.Vitality,
                endurance = statsManager.Endurance,
                strength = statsManager.Strenght
            }
        };

        // Convert user object to JSON
        string json = JsonUtility.ToJson(newUser);

        // Set user data in the database
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);


        //Enable Game Saved UI text
        //GameObject.Find("GameSavedText").GetComponent<TextMeshProUGUI>().text = "GAME SAVED";
    }

    public void DeleteUser() {
        dbReference.Child("users").Child(userId).RemoveValueAsync();
    }

    public void LoadUserData() {
        // Load references
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        statsManager = PlayerStatsManager.instance;
        inventory = GeneralInventory.instance;
        userId = AuthManager.instance.GetFirebaseUserId();

        if (userId != "") {
            
            dbReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted) {
                    // Handle the error
                    Debug.LogError("Failed to load user data: " + task.Exception);
                    return;
                }

                if (task.IsCompleted) {
                    // Parse the JSON data into a User object
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists) {
                        string json = snapshot.GetRawJsonValue();
                        User temUser = new User();
                        
                        temUser = JsonUtility.FromJson<User>(json);

                        this.loadedUser = temUser;

                        // Check if more than two days has passed to nerf the players stats

                        if (!IsCurrentDay(loadedUser.lastLoggedDay)) {
                            DateTime lastLoggedDate = DateTime.ParseExact(loadedUser.lastLoggedDay, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            DateTime currentDay = DateTime.Today;

                            TimeSpan difference = currentDay - lastLoggedDate;

                            if (difference.Days >= 2) {
                                loadedUser.stats.vitality -=1;
                                loadedUser.stats.endurance -=1;
                                loadedUser.stats.strength -=1;
                            }
                        }

                        AssignLoadedUserData(temUser);

                    } else {
                        Debug.LogWarning("User data not found for userId: " + userId);
                    }
                }
            });
        } else {
            Debug.Log("user is empty");
        }

    }

    private bool IsCurrentDay(string dateString) {
        return dateString == DateTime.Today.ToString("yyyy-MM-dd");
    }

    private void AssignLoadedUserData(User loadedUser) {

        foreach (var weaponItem in loadedUser.weaponItems) {
            if (weaponItem != null || weaponItem != "") {
                Debug.Log("Weapon Item: " + weaponItem);
                GeneralInventory.instance.AddItem(ScriptableObjectManager.instance.GetScriptableObject(weaponItem));
            }

        }

        foreach (var quickItem in loadedUser.quickItems) {
            if (quickItem != null || quickItem!="") {
                Debug.Log("Quick Item: " + quickItem);
                GeneralInventory.instance.AddItem(ScriptableObjectManager.instance.GetScriptableObject(quickItem));
            }
        }

        PlayerStatsManager.instance.UpdateUserStatsAndAttritbutes(loadedUser);

    }

    public void SaveGame() {
        CreateOrUpdateUser();
    }

}

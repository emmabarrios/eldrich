using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using System.Collections;
using TMPro;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public FirebaseUser user;

    public string userId;

    public DatabaseReference dbReference;
    public PlayerStatsManager statsManager;
    public GeneralInventory inventory;


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        //user = AuthManager.instance.GetFirebaseUser();
        //userId = AuthManager.instance.GetFirebaseUserId();
        //dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        //statsManager = PlayerStatsManager.instance;
        //inventory = GeneralInventory.instance;
    }

    void Start() {
        //if (instance == null) {
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //} else {
        //    Destroy(gameObject);
        //}

        user = AuthManager.instance.GetFirebaseUser();
        userId = AuthManager.instance.GetFirebaseUserId();
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    public void CreateOrUpdateUser() {



        // Sample data for user creation
        User newUser = new User {
            userId = this.user.UserId,
            weaponItems = inventory.GetWeaponItemsAsStrings(),
            quickItems = inventory.GetQuickItemsAsStrings(),
            exp = statsManager.EarnedExperience,
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
        GameObject.Find("Options Panel").transform.GetChild(1).gameObject.SetActive(true);
    }

    public void DeleteUser() {
        dbReference.Child("users").Child(userId).RemoveValueAsync();
    }

    public void LoadUserData() {
        // Load references
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
                        User loadedUser = JsonUtility.FromJson<User>(json);

                        AssignLoadedUserData(loadedUser);


                        //foreach (var quickItem in loadedUser.quickItems) {
                        //    Debug.Log("Quick Item: " + quickItem);
                        //} 

                        //foreach (var weaponItem in loadedUser.weaponItems) {
                        //    Debug.Log("Quick Item: " + weaponItem);
                        //}

                    } else {
                        Debug.LogWarning("User data not found for userId: " + userId);
                    }
                }
            });
        } else {
            Debug.Log("user is empty");
        }
    }

    private void AssignLoadedUserData(User loadedUser) {
        foreach (var weaponItem in loadedUser.weaponItems) {
            if (weaponItem != null) {
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

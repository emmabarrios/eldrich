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
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public static FirebaseUser user;

    public static string userId;

    public DatabaseReference dbReference;

    public Action<string> OnTaskResult;
    public Action<string> OnAccountTaskResultError;

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

        InitializeFirebaseDatabase();
    }

    public void CreateOrUpdateUser() {
        

        if (user!=null) {

            PlayerStatsManager statsManager = PlayerStatsManager.instance;
            GeneralInventory inventory = GeneralInventory.instance;

            List<string> tempQuickItems = new List<string>();
            List<string> tempWeaponItems = new List<string>();
            List<string> tempLoggedDays = new List<string>();

            tempQuickItems = inventory.GetQuickItemsAsStrings().Where(item => !string.IsNullOrEmpty(item)).ToList();
            tempWeaponItems = inventory.GetWeaponItemsAsStrings().Where(item => !string.IsNullOrEmpty(item)).ToList();
            tempLoggedDays = loadedUser.loggedDays;

            // Gather items from both combat and general inventory
            if (CombatInventory.instance.GetQuickItemsAsStrings() != null) {
                tempQuickItems.AddRange(CombatInventory.instance.GetQuickItemsAsStrings());
            }

            if (CombatInventory.instance.GetEquipedWeaponAsString() != null) {
                tempWeaponItems.Add(CombatInventory.instance.GetEquipedWeaponAsString());
            }

            if (!IsCurrentDay(loadedUser.lastLoggedDay)) {
                tempLoggedDays.Add(DateTime.Today.ToString("yyyy-MM-dd"));
            }

            // Clean weapon item string list
            tempWeaponItems.RemoveAll(s => s == "Sword 1");

            User newUser = new User {
                userId = user.UserId,
                weaponItems = tempWeaponItems,
                quickItems = tempQuickItems,
                loggedDays = tempLoggedDays,
                exp = statsManager.EarnedExperience,
                skillPointCost = statsManager.SkillPointCost,
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
        }
    }

    public void InitializeFirebaseDatabase() {
        user = AuthManager.instance.GetFirebaseUser();
        userId = user.UserId;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (app == null) {
                OnTaskResult?.Invoke("Failed to initialize Firebase.");
                return;
            }

            // Initialize the Firebase Realtime Database
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        });
    }

    public void ResetUserData() {
        User newUser = new User {
            userId = user.UserId,
            weaponItems = null,
            quickItems = null,
            loggedDays = null,
            exp = 0,
            totalTraveledDistance = 0,
            totalDaysLogged = 0,
            lastLoggedDay = null,
            stats = new Stats {
                vitality = 0,
                endurance = 0,
                strength = 0
            }
        };
    }

    public async Task<bool> DeleteUserData() {

        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        CancellationToken token = cts.Token;

        Task tsk = dbReference.Child("users").Child(userId).RemoveValueAsync();

        // Use Task.WhenAny to wait for either tsk or a delay
        var completedTask = await Task.WhenAny(tsk, Task.Delay(Timeout.Infinite, token));

        // Check which task completed
        if (completedTask == tsk) {
            if (!tsk.IsFaulted || !tsk.IsCanceled) {
                user = null;
                return true;
            }
        }

        return false;
    }

    // TEST LATER 
    //public async Task<bool> PerformFirebaseOperation(Task tsk) {

    //    CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    //    CancellationToken token = cts.Token;

    //    Task _tsk = tsk;

    //    // Use Task.WhenAny to wait for either tsk or a delay
    //    var completedTask = await Task.WhenAny(tsk, Task.Delay(Timeout.Infinite, token));

    //    // Check which task completed
    //    if (completedTask == tsk) {
    //        if (!tsk.IsFaulted || !tsk.IsCanceled) {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public async Task<bool> LoadUserData_() {
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        CancellationToken token = cts.Token;

        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = AuthManager.instance.GetFirebaseUserId();

        Task<DataSnapshot> tsk = dbReference.Child("users").Child(userId).GetValueAsync();

        var completedTask = await Task.WhenAny(tsk, Task.Delay(Timeout.Infinite, token));

        if (userId != "") {
            if (completedTask == tsk) {

                // There was a problem with the request
                if (completedTask.IsFaulted) {
                    return false;
                }
                // Parse the JSON data into a User object
                DataSnapshot snapshot = tsk.Result;
                if (snapshot.Exists) {
                    string json = snapshot.GetRawJsonValue();
                    User temUser = new User();

                    temUser = JsonUtility.FromJson<User>(json);

                    this.loadedUser = temUser;

                    Debug.Log(temUser.quickItems);

                    // Check if more than two days has passed to nerf the players stats

                    if (!IsCurrentDay(loadedUser.lastLoggedDay)) {
                        DateTime lastLoggedDate = DateTime.ParseExact(loadedUser.lastLoggedDay, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        DateTime currentDay = DateTime.Today;

                        TimeSpan difference = currentDay - lastLoggedDate;

                        if (difference.Days >= 2) {
                            loadedUser.stats.vitality = (loadedUser.stats.vitality > 1) ? loadedUser.stats.vitality -= 1 : loadedUser.stats.vitality;
                            loadedUser.stats.endurance = (loadedUser.stats.endurance > 1) ? loadedUser.stats.endurance -= 1 : loadedUser.stats.endurance;
                            loadedUser.stats.strength = (loadedUser.stats.strength > 1) ? loadedUser.stats.strength -= 1 : loadedUser.stats.strength;
                        }
                    }

                    AssignLoadedUserData(loadedUser);
                }
                return true;
            } else {
                // Response time ran out
                return false;
            }
        } else {
            return false;
        }

    }
        

    public void LoadUserData() {
        // Load references
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

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
                                loadedUser.stats.vitality = (loadedUser.stats.vitality > 1) ? loadedUser.stats.vitality -= 1 : loadedUser.stats.vitality;
                                loadedUser.stats.endurance = (loadedUser.stats.endurance > 1) ? loadedUser.stats.endurance -= 1 : loadedUser.stats.endurance;
                                loadedUser.stats.strength = (loadedUser.stats.strength > 1) ? loadedUser.stats.strength -= 1 : loadedUser.stats.strength;
                            }
                        }

                        AssignLoadedUserData(loadedUser);

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
            if (weaponItem != null && weaponItem != "") {
                //Debug.LogWarning("Weapon Item: " + weaponItem);
                GeneralInventory.instance.AddItem(ScriptableObjectManager.instance.GetScriptableObject(weaponItem));
            }

        }

        foreach (var quickItem in loadedUser.quickItems) {
            if (quickItem != null && quickItem != "") {
                //Debug.LogWarning("Quick Item: " + quickItem);
                GeneralInventory.instance.AddItem(ScriptableObjectManager.instance.GetScriptableObject(quickItem));
            }
        }

        PlayerStatsManager.instance.UpdateUserStatsAndAttritbutes(loadedUser);

    }

    public void SaveGame() {
        CreateOrUpdateUser();
    }


}

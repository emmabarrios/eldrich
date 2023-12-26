using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string userId;
    public DatabaseReference dbReference;

    void Start() {
        userId = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //void Start() {
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
    //    {
    //        FirebaseApp app = FirebaseApp.DefaultInstance;
    //        userId = SystemInfo.deviceUniqueIdentifier;

    //        if (task.Exception != null) {
    //            Debug.LogError($"Failed to initialize Firebase: {task.Exception}");
    //            return;
    //        }

    //        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    //    });

    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
    //        var dependencyStatus = task.Result;
    //        if (dependencyStatus == DependencyStatus.Available) {
    //            FirebaseApp app = FirebaseApp.DefaultInstance;

    //            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    //            Debug.Log("passed?");
    //            //"https://eldrich-78666-default-rtdb.firebaseio.com/"


    //            // Set a flag here to indicate whether Firebase is ready to use by your app.
    //        } else {
    //            Debug.LogError(System.String.Format(
    //                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
    //            // Firebase Unity SDK is not safe to use here.
    //        }
    //    });
    //}

    public void CreateUser() {
        User testUser = new User("dummy", "dummy@mail.com");
        Debug.Log(testUser.ToString());
        string json = JsonUtility.ToJson(testUser);

        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

}

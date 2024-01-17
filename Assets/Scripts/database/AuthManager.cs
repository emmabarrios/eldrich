using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using System;
using System.Threading;

public class AuthManager : MonoBehaviour {

    public static AuthManager instance;

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    public Action<string> OnRegisterTaskResult; 
    public Action<string> OnDeleteAccountTaskResultSuccess; 
    public Action<string> OnDeleteAccountTaskResultError; 

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                //If they are avalible Initialize Firebase
                InitializeFirebaseAuth();
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    public void InitializeFirebaseAuth() {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    public IEnumerator Login(string email, string password) {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null) {
            //If there are errors handle them
            //Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";

            switch (errorCode) {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            OnRegisterTaskResult?.Invoke(message);
        } else {
            User = LoginTask.Result.User;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
        }
    }

    public IEnumerator Register(string email, string password, string passwordConfirmation, string username) {
        if (username == "") {
            OnRegisterTaskResult?.Invoke("Missing Username");
        } else if (password != passwordConfirmation) {
            //If the password does not match show a warning
            OnRegisterTaskResult?.Invoke("Password Does Not Match!");
        } else {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null) {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode) {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email Format";
                        break;

                }
                OnRegisterTaskResult?.Invoke(message);
            } else {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null) {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null) {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        OnRegisterTaskResult?.Invoke("Username Set Failed!");
                    } else {
                        //Username is now set
                        OnRegisterTaskResult?.Invoke("User registered successfully!");
                    }
                }
            }
        }

    }

    public string GetFirebaseUserId() {
        if (auth.CurrentUser != null) {
            return auth.CurrentUser.UserId;
        }
        return null;
    }

    public FirebaseUser GetFirebaseUser() {
        return auth.CurrentUser;
    }

    public async Task<bool> DeleteAccount() {

        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        CancellationToken token = cts.Token;

        Task tsk = auth.CurrentUser.DeleteAsync();

        // Use Task.WhenAny to wait for either tsk or a delay
        var completedTask = await Task.WhenAny(tsk, Task.Delay(Timeout.Infinite, token));

        // Check which task completed
        if (completedTask == tsk) {
            if (!tsk.IsFaulted || !tsk.IsCanceled) {
                return true;
            }
        }

        return false;
    }
}
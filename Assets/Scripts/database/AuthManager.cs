using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System;
using System.Threading;

public class AuthManager : MonoBehaviour {

    public static AuthManager instance;

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

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                InitializeFirebaseAuth();
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    public void InitializeFirebaseAuth() {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    public IEnumerator Login(string email, string password) {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null) {
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
        }
    }

    public IEnumerator Register(string email, string password, string passwordConfirmation, string username) {
        if (username == "") {
            OnRegisterTaskResult?.Invoke("Missing Username");
        } else if (password != passwordConfirmation) {
            OnRegisterTaskResult?.Invoke("Password Does Not Match!");
        } else {
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
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
                User = RegisterTask.Result.User;

                if (User != null) {
                    UserProfile profile = new UserProfile { DisplayName = username };
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null) {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        OnRegisterTaskResult?.Invoke("Username Set Failed!");
                    } else {
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

        var completedTask = await Task.WhenAny(tsk, Task.Delay(Timeout.Infinite, token));

        if (completedTask == tsk) {
            if (!tsk.IsFaulted || !tsk.IsCanceled) {
                return true;
            }
        }

        return false;
    }
}
using System.Collections;
using UnityEngine;
using TMPro;


public class SessionPanel : MonoBehaviour
{
    //Login variables
    [Header("Login Panel")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public UnityEngine.UI.Button loginButton;


    [Header("Registration Panel")]
    public TMP_InputField emailRegisterField;
    public TMP_InputField userNameField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordConfirmField;
    public UnityEngine.UI.Button registerButton;

    [Header("State Message")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;


    private void Start() {
        StartCoroutine(WaitForResources());
    }


    private void Login() {
        StartCoroutine(LoginCoroutine());
    }

    private void Register() {
        StartCoroutine(RegisterCoroutine());
    }

    private void ShowSessionStateMessage(string msg) {
        if (messagePanel!=null) {
            messagePanel.SetActive(true);
            messageText.text = msg;
        }
    }

    private IEnumerator WaitForResources() {

        LoadingPanel.instance.ShowLoadingScreen();

        AuthManager authManager = null;

        // Wait until authManager is not null
        while (authManager == null) {
            authManager = AuthManager.instance;
            yield return null; // Wait for the next frame
        }

        AuthManager.instance.OnRegisterTaskResult += ShowSessionStateMessage;

        loginButton.onClick.AddListener(() => { Login(); });
        registerButton.onClick.AddListener(() => { Register(); });

        LoadingPanel.instance.HideLoadingScreen();
    }

    private IEnumerator LoginCoroutine() {
        // Reset message state
        messageText.text = "";

        LoadingPanel.instance.ShowLoadingScreen();

        // Wait for the login operation to complete
        yield return AuthManager.instance.Login(emailLoginField.text, passwordLoginField.text);

        // Check if there is an error message
        if (string.IsNullOrEmpty(messageText.text)) {
            // No error message, continue with your logic
            Loader.Load(Loader.Scene.Overworld);
        } else {
            LoadingPanel.instance.HideLoadingScreen();
        }
    }

    private IEnumerator RegisterCoroutine() {
        LoadingPanel.instance.ShowLoadingScreen();

        // Wait for the login operation to complete
        yield return AuthManager.instance.Register(emailRegisterField.text, passwordLoginField.text, passwordConfirmField.text, userNameField.text);

        LoadingPanel.instance.HideLoadingScreen();
    }
}

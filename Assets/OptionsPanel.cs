using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class OptionsPanel : MonoBehaviour
{

    [SerializeField]
    private GameObject deleteAccountPanel;
    [SerializeField]
    private GameObject newGamePanel;


    [Header("State MessageS")]
    public GameObject operationResultMessagePanel;
    public TextMeshProUGUI messageText;


    public void SaveGame() {
        DatabaseManager.instance.CreateOrUpdateUser();
    }

    public async void DeleteAccount() {
        deleteAccountPanel.SetActive(false);

        LoadingPanel.instance.ShowLoadingScreen();

        bool wasOperationSuccessful = await AuthManager.instance.DeleteAccount();

        if (wasOperationSuccessful) {

            bool wasSecondOperationSuccessful = await DatabaseManager.instance.DeleteUserData();

            if (wasSecondOperationSuccessful) {
                Loader.Load(Loader.Scene.TitleScene);
            } else {
                ShowOperationResultMessage("An error has occurred while deleting your account, please check your connection and try again.");
            }

        } else {
            ShowOperationResultMessage("An error has occurred, please check your connection and try again.");
        }

        LoadingPanel.instance.HideLoadingScreen();
    }

    public async void StartNewGame() {
        newGamePanel.SetActive(false);

        LoadingPanel.instance.ShowLoadingScreen();

        bool wasOperationSuccessful = await DatabaseManager.instance.DeleteUserData();

        if (wasOperationSuccessful) {
            Loader.Load(Loader.Scene.TitleScene);
        } else {
            ShowOperationResultMessage("An error has occurred, please check your connection and try again.");
        }

        LoadingPanel.instance.HideLoadingScreen();
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void CloseSession() {
        Destroy(DatabaseManager.instance.gameObject);
        Destroy(AuthManager.instance.gameObject);
        Loader.Load(Loader.Scene.TitleScene);
    }

    private void Start() {
        StartCoroutine(WaitForResources());
    }

        
    private void ShowOperationResultMessage(string msg) {
        if (operationResultMessagePanel != null) {
            operationResultMessagePanel.SetActive(true);
            messageText.text = msg;
        }
    }

    public void ReturnToTitleScreen() {
        Loader.Load(Loader.Scene.TitleScene);
    }

    private IEnumerator WaitForResources() {

        LoadingPanel.instance.ShowLoadingScreen();

        DatabaseManager databaseManager = null;

        // Wait until authManager is not null
        while (databaseManager == null) {
            databaseManager = DatabaseManager.instance;
            yield return null; // Wait for the next frame
        }

        DatabaseManager.instance.OnTaskResult += ShowOperationResultMessage;
        LoadingPanel.instance.HideLoadingScreen();
    }
}

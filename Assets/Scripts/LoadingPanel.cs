using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public static LoadingPanel instance;
    

    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private TextMeshProUGUI loadingMessage;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }


    public void ShowLoadingScreen() {
        if (!loadingScreenPanel.activeSelf) {
            loadingScreenPanel.SetActive(true);
        }
    }
    public void HideLoadingScreen() {
        if (loadingScreenPanel.activeSelf) {
            loadingMessage.text = "";
            loadingScreenPanel.SetActive(false);
        }
    }

    public void UpdateLoadingMessage(string msg) {
        loadingMessage.text = msg;
    }
}

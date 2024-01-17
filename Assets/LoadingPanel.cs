using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public static LoadingPanel instance;

    [SerializeField]
    private GameObject loadingScreenPanel;

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
            loadingScreenPanel.SetActive(false);
        }
    }
}

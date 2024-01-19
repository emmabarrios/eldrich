using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadErrorPanel : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button retryButton;
    [SerializeField] private UnityEngine.UI.Button closeAppButton;

    private void OnEnable() {
        retryButton.onClick.AddListener(() => GameManager.instance.LoadUserData());
        closeAppButton.onClick.AddListener(() => Application.Quit());
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Location;

public class OverworldUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventUiPanel;
    [SerializeField] private GameObject errorLoadingPanel;


    // Start is called before the first frame update
    public void DisplayStartEventPanel() {
        eventUiPanel.SetActive(true);
    }

    public void ToggleErrorLoadingPanel() {
        errorLoadingPanel.SetActive(!errorLoadingPanel.activeSelf);
    }

}

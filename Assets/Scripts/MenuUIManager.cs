using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventPanelUserInRange;
    [SerializeField] private GameObject eventPanelUserNotInRange;
    public bool isUiPanelActive;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayStartEventPanel() {
        if (isUiPanelActive == false) {
            eventPanelUserInRange.SetActive(true);
            isUiPanelActive = true;
        }
    }
    
    public void DisplayUserNotInRangeEventPanel() {
        if (isUiPanelActive == false) {
            eventPanelUserNotInRange.SetActive(true);
            isUiPanelActive = true;
        }
    }

    public void CloseButtonClick() {
        eventPanelUserInRange.SetActive(false);
        eventPanelUserNotInRange.SetActive(false);
        isUiPanelActive = false;

    }
}

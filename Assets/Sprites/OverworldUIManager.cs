using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Location;

public class OverworldUIManager : MonoBehaviour
{
    [SerializeField] private GameObject eventUiPanel;
    //[SerializeField] private GameObject eventPanelUserNotInRange;
    public bool isEventUiPanelActive;


    // Start is called before the first frame update
    public void DisplayStartEventPanel() {
        if (isEventUiPanelActive == false) {
            eventUiPanel.SetActive(true);
            isEventUiPanelActive = true;
        }
    }

    //public void DisplayUserNotInRangeEventPanel() {
    //    if (isEventUiPanelActive == false) {
    //        eventPanelUserNotInRange.SetActive(true);
    //        isEventUiPanelActive = true;
    //    }
    //}


    public void UpdateStatsUIContent() {

        int availableSkillPoints = PlayerStatsManager.instance.SkillPoints;
        bool canIncrease = (availableSkillPoints > 0);

        // Update Health
        GameObject.Find("Health Stat Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.Health.ToString();
        // Update Stamina
        GameObject.Find("Stamina Stat Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.Stamina.ToString();
        // Update Damage
        GameObject.Find("Damage Stat Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.Damage.ToString();
        // Update Defense
        GameObject.Find("Defense Stat Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.Defense.ToString();

        // Update next 100m card
        //double totalDistance = GameObject.Find("AndroidDeviceLocationProvider").GetComponent<DeviceLocationProviderAndroidNative>().totalTraveledDistance;
        //double nextPoint = 100 - totalDistance;
        //GameObject.Find("Travel Distance Card").transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = nextPoint.ToString();


        // Update experience balance
        GameObject.Find("Earned Exp Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.EarnedExperience.ToString();
        GameObject.Find("Skill Point Cost Card").transform.GetChild(3).GetComponent<Text>().text = PlayerStatsManager.instance.SkillPointCost.ToString();

        // Reference to the accept stats changes button
        UnityEngine.UI.Button acceptStatChangesButton = GameObject.Find("Accept Changes Button").GetComponent<UnityEngine.UI.Button>();

        // Toggle stats modifier buttons state
        foreach (CharacterStatModifierCard statsCard in GameObject.Find("Stats Panel").GetComponentsInChildren<CharacterStatModifierCard>()) {
            
            if (!canIncrease) {
                statsCard.transform.GetChild(3).GetComponent<UnityEngine.UI.Button>().interactable = false;
            } else {
                statsCard.transform.GetChild(3).GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            bool canDecrease = (statsCard.CurrentValue > statsCard.LastValue);

            if (!canDecrease) {
                statsCard.transform.GetChild(4).GetComponent<UnityEngine.UI.Button>().interactable = false;
            } else {
                statsCard.transform.GetChild(4).GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }

        // Enable Accept Stat Changes Button if atributes are different than their last value
        foreach (CharacterStatModifierCard statsCard in GameObject.Find("Stats Panel").GetComponentsInChildren<CharacterStatModifierCard>()) {
            if (statsCard.LastValue != statsCard.CurrentValue) {
                acceptStatChangesButton.interactable = true;
                return;
            }
        }

        //Deactivate the level up button if there is no change in stats or if the stats are already setled
        acceptStatChangesButton.interactable = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptStatChangesButton : MonoBehaviour
{
    public void SettleStatsValues() {
        OverworldUIManager overworldUIManager = GameObject.Find("Canvas").GetComponent<OverworldUIManager>();

        foreach (CharacterStatModifierCard statsCard in this.transform.parent.GetComponentsInChildren<CharacterStatModifierCard>()) {
            statsCard.SubmitValue();
        }

        //PlayerStatsManager.instance.UpdateExperience(0);
        //PlayerStatsManager.instance.UpdateLastStats();

        overworldUIManager.UpdateStatsUIContent();

        // Update database
        DatabaseManager.instance.CreateOrUpdateUser();
    }

    private void Start() {
        //this.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

}

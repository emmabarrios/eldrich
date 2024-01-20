using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatModifierCard : MonoBehaviour
{
    [SerializeField] Text currentValueText;
    [SerializeField] TextMeshProUGUI cardTitle;
    [SerializeField] UnityEngine.UI.Button addButton;
    [SerializeField] UnityEngine.UI.Button subtractButton;

    // Last value is a dummy data, it should be stored in a database elsewhere
    [SerializeField] int lastValue = 1;

    [SerializeField] int currentValue;
    public int LastValue { get { return lastValue; } set { lastValue = value; } }
    public int CurrentValue { get { return currentValue; } set { currentValue = value; } }

    private StatsPanel statsPanel;

    public enum CardStat {
        Vitality,
        Endurance,
        Strenght
    }

    public CardStat cardStat;

    private void OnEnable() {
        cardTitle.text = cardStat.ToString();

        LastValue = PlayerStatsManager.instance.GetLastValue(cardStat);

        CurrentValue = LastValue;

        currentValueText.text = CurrentValue.ToString();

        statsPanel = GameObject.Find("Stats Panel").GetComponentInChildren<StatsPanel>();
    }


    public void AddLevel() {
        int availableSkillPoints = PlayerStatsManager.instance.SkillPoints;

        if (availableSkillPoints > 0) {
            CurrentValue += 1;

            currentValueText.text = CurrentValue.ToString();

            PlayerStatsManager.instance.EarnedExperience -= PlayerStatsManager.instance.SkillPointCost;
            PlayerStatsManager.instance.SkillPointCost += 1;

            PlayerStatsManager.instance.UpdateStat(cardStat);

            PlayerStatsManager.instance.UpdateExperience(0);
            PlayerStatsManager.instance.UpdateLastStats();

            statsPanel.UpdateUIpanel();
        }
        
    }
    
    public void SubtractLevel() {

        if (CurrentValue > LastValue) {
            CurrentValue -= 1;

            currentValueText.text = CurrentValue.ToString();

            PlayerStatsManager.instance.SkillPointCost -= 1;
            PlayerStatsManager.instance.EarnedExperience += PlayerStatsManager.instance.SkillPointCost;

            PlayerStatsManager.instance.UpdateStat(cardStat);

            PlayerStatsManager.instance.UpdateExperience(0);
            PlayerStatsManager.instance.UpdateLastStats();

            statsPanel.UpdateUIpanel();
        }
        
    }

    public void SubmitValue() {
        //subtractButton.interactable = false;

        switch (cardStat) {
            case CardStat.Vitality:
                PlayerStatsManager.instance.Vitality = CurrentValue;
                break;
            case CardStat.Endurance:
                PlayerStatsManager.instance.Endurance = CurrentValue;
                break;
            case CardStat.Strenght:
                PlayerStatsManager.instance.Strenght = CurrentValue;
                break;
            default:
                break;
        }

        LastValue = PlayerStatsManager.instance.GetLastValue(cardStat);
        CurrentValue = LastValue;
    }

    public void ToggleAddButton(bool state) {
        addButton.interactable = state;
    }

    public void ToggleSubtractButton(bool state) {
        subtractButton.interactable = state;
    }

}

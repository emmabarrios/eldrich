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

    private OverworldUIManager overworldUIManager;

    public enum CardStat {
        Vitality,
        Endurance,
        Strenght
    }

    public CardStat cardStat;

    private void OnEnable() {
        cardTitle.text = cardStat.ToString();
        addButton.onClick.AddListener(() => AddLevel());
        subtractButton.onClick.AddListener(() => SubtractLevel());

        LastValue = PlayerStatsManager.instance.GetLastValue(cardStat);

        CurrentValue = LastValue;

        currentValueText.text = CurrentValue.ToString();

        overworldUIManager = GameObject.Find("Canvas").GetComponent<OverworldUIManager>();
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
        }
        overworldUIManager.UpdateStatsUIContent();
    }
    
    public void SubtractLevel() {

        if (CurrentValue > LastValue) {
            CurrentValue -= 1;

            currentValueText.text = CurrentValue.ToString();

            addButton.interactable = true;

            PlayerStatsManager.instance.EarnedExperience += PlayerStatsManager.instance.SkillPointCost;
            PlayerStatsManager.instance.SkillPointCost -= 1;

            PlayerStatsManager.instance.UpdateStat(cardStat);

            PlayerStatsManager.instance.UpdateExperience(0);
            PlayerStatsManager.instance.UpdateLastStats();
        }
        overworldUIManager.UpdateStatsUIContent();
    }

    public void SubmitValue() {
        subtractButton.interactable = false;

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
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    [Header("stats cards text")]
    public Text healthText;
    public Text staminaText;
    public Text damageText;
    public Text defenseText;

    [Header("Exp and skills point cards text")]
    public Text expPointsText;
    public Text skillPointsText;

    [Header("Accept changes button")]
    public UnityEngine.UI.Button acceptStatChangesButton;

    [Header("Stats modifier cards")]
    public CharacterStatModifierCard[] statCards;

    private void OnEnable() {
        UpdateUIpanel();

    }

    private void Awake() {
        acceptStatChangesButton.onClick.AddListener(() => SettleStatsValues());
    }

    
    public void UpdateUIpanel() {
        PlayerStatsManager psm = PlayerStatsManager.instance;

        int availableSkillPoints = psm.EarnedExperience;
        int skillPointCost = psm.SkillPointCost;

        bool canIncrease = (skillPointCost <= availableSkillPoints);

        //Debug.Log(canIncrease);

        // Update point cards
        expPointsText.text = psm.EarnedExperience.ToString();
        skillPointsText.text = psm.SkillPointCost.ToString();

        // Update stat cards
        healthText.text = psm.Health.ToString();
        staminaText.text = psm.Stamina.ToString();
        damageText.text = psm.Damage.ToString();
        defenseText.text = psm.Defense.ToString();

        // Toggle stats modifier buttons state
        foreach (CharacterStatModifierCard statsCard in statCards) {

            bool canDecrease = (statsCard.CurrentValue > statsCard.LastValue);

            if (!canIncrease) {
                statsCard.ToggleAddButton(false);
            } else {
                statsCard.ToggleAddButton(true);
            }


            if (!canDecrease) {
                statsCard.ToggleSubtractButton(false);
            } else {
                statsCard.ToggleSubtractButton(true);
            }
        }

        // Enable Accept Stat Changes Button if atributes are different than their last value
        foreach (CharacterStatModifierCard statsCard in statCards) {
            if (statsCard.LastValue != statsCard.CurrentValue) {
                acceptStatChangesButton.interactable = true;
                return;
            }
        }

        //Deactivate the level up button if there is no change in stats or if the stats are already setled
        acceptStatChangesButton.interactable = false;
    }

    public void SettleStatsValues() {

        foreach (CharacterStatModifierCard statsCard in statCards) {
            statsCard.SubmitValue();
        }

        UpdateUIpanel();

        DatabaseManager.instance.SaveGame();

    }

}

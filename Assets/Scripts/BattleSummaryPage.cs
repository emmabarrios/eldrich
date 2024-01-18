using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BattleSummaryPage : MonoBehaviour
{
    [SerializeField] private Text textArea;

    UnityEngine.UI.Button button;
    private void OnEnable() {
        button = GetComponentInChildren<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => Loader.Load(Loader.Scene.Overworld));

        UpdateTextContent();
    }

    private void UpdateTextContent() {

        if (textArea != null) {
            List<Item> itemlist = GameManager.instance.GetLoot();

            // Update text for dropped items
            foreach (Item item in itemlist) {
                textArea.text += $"{item._name}.\n";
            }

            textArea.text += "\n";
            textArea.text += "\n";
            textArea.text += "\n";

            // Update text for gained experience
            textArea.text += $"{GameManager.instance.GetCombatExperience()} exp points.";
        }
    }
}

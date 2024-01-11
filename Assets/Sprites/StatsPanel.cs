using UnityEngine;
using TMPro;



public class StatsPanel : MonoBehaviour
{
    private void OnEnable() {
        OverworldUIManager overworldUIManager = GameObject.Find("Canvas").GetComponent<OverworldUIManager>();
        overworldUIManager.UpdateStatsUIContent();
    }
}

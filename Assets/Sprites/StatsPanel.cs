using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    private bool wasEnabled = false;


    private void OnEnable() {
        OverworldUIManager overworldUIManager = GameObject.Find("Canvas").GetComponent<OverworldUIManager>();
        overworldUIManager.UpdateStatsUIContent();
    }
}

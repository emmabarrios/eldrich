using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsPanel : MonoBehaviour
{
    private void OnDisable() {
        this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AppStatusMessagePanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txt;

    public void SetText(string text) {
        txt.text = text;
    }
}

using UnityEngine;
using TMPro;


public class AppStatusMessagePanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txt;

    public void SetText(string text) {
        txt.text = text;
    }
}

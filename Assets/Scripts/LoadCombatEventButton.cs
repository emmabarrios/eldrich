using UnityEngine;
using UnityEngine.UI;

public class LoadCombatEventButton : MonoBehaviour
{
    public UnityEngine.UI.Button button = null;

    private void OnEnable() {
        button = this.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => Loader.Load(Loader.Scene.MainScene));
    }
}

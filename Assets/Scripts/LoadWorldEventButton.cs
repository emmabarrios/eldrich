using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWorldEventButton : MonoBehaviour
{
    private UnityEngine.UI.Button button = null;
    private void OnEnable() {
        button = this.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => Loader.Load(Scene.Overworld));
    }
    
}

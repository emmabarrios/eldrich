using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWorldEventButton : MonoBehaviour
{
    public UnityEngine.UI.Button button = null;
    private void OnEnable() {
        button = this.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => Loader.Load(Loader.Scene.Overworld));
    }
    
}

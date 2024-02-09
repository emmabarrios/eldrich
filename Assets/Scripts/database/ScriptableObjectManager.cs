using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour {
    public static ScriptableObjectManager instance; 

    private Dictionary<string, Item> scriptableObjectsDictionary = new Dictionary<string, Item>();

    public Item[] scriptableObjects;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeDictionary();
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary() {
        scriptableObjects = Resources.LoadAll<Item>("ScriptableObjects/Items");

        foreach (Item so in scriptableObjects) {
            scriptableObjectsDictionary.Add(so.name, so);
        }
    }

    public Item GetScriptableObject(string name) {
        if (name!="" && name != null) {
            if (scriptableObjectsDictionary.TryGetValue(name, out Item so)) {
                return so;
            }
        }

        Debug.LogWarning($"Scriptable Object with name '{name}' not found.");
        return null;
    }

}

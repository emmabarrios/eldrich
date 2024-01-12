using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour {
    public static ScriptableObjectManager instance; // Singleton instance

    // Dictionary to store Scriptable Objects based on name
    private Dictionary<string, Item> scriptableObjectsDictionary = new Dictionary<string, Item>();

    public Item[] scriptableObjects;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Populate the dictionary with your Scriptable Objects
            InitializeDictionary();
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary() {
        scriptableObjects = Resources.LoadAll<Item>("ScriptableObjects");

        foreach (Item so in scriptableObjects) {
            // Add each Scriptable Object to the dictionary using its name
            scriptableObjectsDictionary.Add(so.name, so);
        }
    }

    // Retrieve a Scriptable Object by name
    public Item GetScriptableObject(string name) {
        if (scriptableObjectsDictionary.TryGetValue(name, out Item so)) {
            return so;
        }

        Debug.LogWarning($"Scriptable Object with name '{name}' not found.");
        return null;
    }

}

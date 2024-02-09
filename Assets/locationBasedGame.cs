using Mapbox.Unity.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class locationBasedGame : MonoBehaviour
{
    public string sceneName = "nooone";

    public static locationBasedGame instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {

        //gameObject.SetActive(false);

        if (scene.name == "Overworld") {
            sceneName = scene.name;
            if (gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
        }else if (scene.name == "MainScene") {
            sceneName = scene.name;
            if (gameObject.activeSelf == true) {
                gameObject.SetActive(false);
            }
        }
    }
}

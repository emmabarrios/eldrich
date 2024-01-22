using UnityEngine.SceneManagement;
using System;

public static class Loader
{
    public static Action onLoaderCallback;
       public static void Load(Scene scene) {
        // Set the loader callback action to load the target scene
        onLoaderCallback = () => {
            SceneManager.LoadScene(scene.ToString());
        };

        // Load the loading scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback() {
        if (onLoaderCallback != null) {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}

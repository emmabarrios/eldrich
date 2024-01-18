using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    private static float globalVolume = 1.0f; // Default volume is 100%

    public static float GlobalVolume {
        get { return globalVolume; }
        set {
            globalVolume = Mathf.Clamp01(value); // Ensure volume is between 0 and 1
            UpdateAllAudioSources();
        }
    }

    private static void UpdateAllAudioSources() {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources) {
            audioSource.volume = globalVolume;
        }
    }
}

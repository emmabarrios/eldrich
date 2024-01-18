using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Start() {
        slider = GetComponent<Slider>();
        StartCoroutine(WaitForResources());
    }

    private void OnSliderValueChanged(float value) {
        GameManager.instance.GlobalVolume = value;
    }

    private IEnumerator WaitForResources() {

        GameManager authManager = null;

        // Wait until authManager is not null
        while (authManager == null) {
            authManager = GameManager.instance;
            yield return null; // Wait for the next frame
        }

        if (slider != null) {
            slider.value = GameManager.instance.GlobalVolume;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        } else {
            Debug.LogError("No Slider component found on this GameObject.");
        }
    }
}

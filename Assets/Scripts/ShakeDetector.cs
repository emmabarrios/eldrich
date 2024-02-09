using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeDetector : MonoBehaviour {
    public float shakeThreshold = 2.0f; // Adjust this value based on testing
    public Action OnShake;

    //void Update() {
    //    // Check if the device supports accelerometer
    //    if (SystemInfo.supportsAccelerometer) {
    //        // Get acceleration data
    //        Vector3 acceleration = Input.acceleration;

    //        // Calculate the magnitude of the acceleration change
    //        float accelerationMagnitude = acceleration.magnitude;

    //        // Check if the device was shaken abruptly
    //        if (accelerationMagnitude > shakeThreshold) {
    //            Debug.Log("Device shaken!");
    //            OnShake?.Invoke();
    //        }
    //    } else {
    //        Debug.LogError("Accelerometer is not supported on this device.");
    //    }
    //}
}
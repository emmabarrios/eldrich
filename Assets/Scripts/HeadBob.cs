using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob: MonoBehaviour
{
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float bobAmplitude = 0.05f;

    //private Player player;
    [SerializeField] private Controller playerController = null;
    private Transform cameraTransform;
    private Vector3 initialCameraPosition;
    private float timer;

    private void Start() {
        cameraTransform = GetComponentInChildren<Camera>().transform;
        initialCameraPosition = cameraTransform.localPosition;
        timer = 0f;
    }

    private void Update() {
        // Check if the player is moving
        //if (player.IsWalking) {
            // Update the head bob animation
           // UpdateHeadBob();
        //} else {
            // Reset the head bob position when the player stops moving
            ResetHeadBob();
        //}
    }

    private void UpdateHeadBob() {
        // Calculate the horizontal and vertical head bob offsets
        float horizontalOffset = Mathf.Sin(timer * bobFrequency) * bobAmplitude;
        float verticalOffset = Mathf.Cos(timer * bobFrequency * 2f) * bobAmplitude * 0.5f;

        // Apply the head bob offsets to the camera's local position
        Vector3 targetPosition = initialCameraPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * 10f);

        // Increment the timer
        timer += Time.deltaTime * playerController.CurrentSpeed;
    }

    private void ResetHeadBob() {
        // Reset the camera's local position to the initial position
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCameraPosition, Time.deltaTime * 10f);

        // Reset the timer
        timer = 0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;  
    public float maxDistance = 10f;

    void Update() {
        // Ensure that there is a target assigned
        if (target == null) {
            Debug.LogWarning("No target assigned to the OrbitCamera.");
            return;
        }

        // Always look at the target
        transform.LookAt(target);

        // Check for touch input to rotate and zoom the camera
        if (Input.touchCount == 1) {
            // Rotate the camera based on touch input
            float touchDeltaX = Input.GetTouch(0).deltaPosition.x * rotationSpeed * Time.deltaTime;
            float touchDeltaY = Input.GetTouch(0).deltaPosition.y * zoomSpeed * Time.deltaTime;

            // Rotate the camera around the target based on touch input
            transform.RotateAround(target.position, Vector3.up, touchDeltaX);

            // Zoom the camera based on touch input
            float newDistance = Mathf.Clamp(Vector3.Distance(transform.position, target.position) - touchDeltaY, minDistance, maxDistance);

            // Set the new position of the camera
            transform.position = target.position - transform.forward * newDistance;

        }
    }


}

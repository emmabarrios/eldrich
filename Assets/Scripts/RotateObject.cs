using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 60f; // Speed of rotation in degrees per second

    private void Update() {
        // Calculate the rotation amount based on time and speed
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply the rotation around the Y-axis
        transform.Rotate(Vector3.up, rotationAmount);
    }
}

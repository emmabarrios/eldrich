using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform targetObject;  // The object to follow
    public float distance = 5.0f;   // The desired distance between the objects

    void Update() {
        if (targetObject != null) {
            // Calculate the desired position based on the target object's position and distance
            Vector3 desiredPosition = targetObject.position - targetObject.forward * distance;

            // Move the current object to the desired position
            transform.position = desiredPosition;

            // Make the current object look at the target object
            transform.LookAt(targetObject);
        }
    }
}

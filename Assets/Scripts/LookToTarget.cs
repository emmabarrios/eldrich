using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToTarget : MonoBehaviour
{
    [SerializeField] private Transform targetObject = null;
    public float rotationSpeed;

    private void FixedUpdate() {
       // transform.LookAt(targetObject);
        // Smoothly rotate the camera holder towards the target object
        Quaternion targetRotation = Quaternion.LookRotation(targetObject.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}

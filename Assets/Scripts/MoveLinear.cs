using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinear : MonoBehaviour
{
    // Reference to the RectTransform component of the UI element
    private RectTransform rectTransform;

    // Initial position of the UI element
    private Vector2 initialPosition;

    // Distance to move from the initial position
    public float distance = 100f;

    // Speed of the movement
    public float speed = 1f;

    // Direction of the movement (1 for right, -1 for left)
    private int direction = 1;

    private bool isActive = false;

    public bool IsActive {
        get { return isActive; }
        set { isActive = value; }
    }

    private void Start() {
        // Get a reference to the RectTransform component
        rectTransform = GetComponent<RectTransform>();

        // Store the initial position of the UI element
        initialPosition = rectTransform.anchoredPosition;
    }

    private void Update() {

        if (isActive) {
            // Calculate the new position based on the direction, distance, and speed
            float newPosition = initialPosition.x + (direction * distance * Mathf.Sin(Time.time * speed));

            // Update the anchored position of the UI element
            rectTransform.anchoredPosition = new Vector2(newPosition, initialPosition.y);
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WithdrawItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    // Minimum swipe distance threshold
    public float swipeThreshold = 50f;

    private Camera cam;

    // Reference to the RectTransform component of the UI element
    private RectTransform rectTransform;

    // Flag to indicate if a swipe is in progress
    private bool isSwipeInProgress = false;

    // Initial touch position
    private Vector2 initialTouchPosition;

    private void Awake() {
        // Get a reference to the RectTransform component
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Check if the initial touch occurred within the UI element
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, cam)) {
            // Set the flag to indicate a swipe is in progress
            isSwipeInProgress = true;

            // Store the initial touch position
            initialTouchPosition = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        // Check if a swipe is in progress
        if (isSwipeInProgress) {
            // Calculate the distance between the initial touch position and the current touch position
            float swipeDistance = eventData.position.y - initialTouchPosition.y;

            // Trigger the appropriate swipe event based on the swipe distance
            if (swipeDistance > swipeThreshold) {
                Debug.Log("shiiiun");
            } 

            // Reset the swipe in progress flag
            isSwipeInProgress = false;
        }
    }
}

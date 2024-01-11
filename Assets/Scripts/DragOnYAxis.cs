using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragOnYAxis : MonoBehaviour, IPointerDownHandler, IDragHandler {
    // Reference to the RectTransform component of the UI element
    private RectTransform rectTransform;

    // For the dragging part of the script
    private Camera cam;

    // Flag to indicate if dragging is allowed
    //private bool isDragging = false;

    public bool isActive = false;

    // Initial position of the UI element
    public Vector2 initialPosition;

    private Canvas canvas;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        initialPosition = rectTransform.position;
        OnDrag(eventData);
    }

    //public void OnDrag(PointerEventData eventData) {
    //    Vector2 position;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        (RectTransform)canvas.transform,
    //        eventData.position,
    //        canvas.worldCamera,
    //        out position);
    //    transform.position = canvas.transform.TransformPoint(position);
    //}

    //public void OnDrag(PointerEventData eventData) {
    //    Vector2 position;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        (RectTransform)canvas.transform,
    //        eventData.position,
    //        canvas.worldCamera,
    //        out position);

    //    Vector2 newPosition = canvas.transform.TransformPoint(position);
    //    newPosition.x = transform.position.x; // Lock the X position
    //    newPosition.y = Mathf.Max(newPosition.y, transform.position.y); // Restrict to positive Y-axis

    //    transform.position = newPosition;
    //}

    public void OnDrag(PointerEventData eventData) {

        if (isActive) {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                eventData.position,
                canvas.worldCamera,
                out position);

            Vector3 newPosition = canvas.transform.TransformPoint(position);
            newPosition.x = initialPosition.x; // Lock the X position

            if (newPosition.y >= initialPosition.y) {
                transform.position = newPosition;
            } else {
                transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
            }
        }
        
    }
}

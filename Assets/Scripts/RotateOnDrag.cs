using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateOnDrag : MonoBehaviour, IDragHandler, IEndDragHandler {
    public Transform rotableObject;
    public float rotationSpeed = 5f;

    private bool isDragging = false;

    void Update() {
        if (isDragging) {
            RotateObject();
            Debug.Log("dragging");
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (RectTransformUtility.RectangleContainsScreenPoint(this.GetComponent<RectTransform>(), eventData.position)) {
            isDragging = true;
        } else {
            isDragging = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        isDragging = false;
    }

    void RotateObject() {
        float rotationAmount = Input.GetTouch(0).deltaPosition.x * rotationSpeed * Time.deltaTime;
        rotableObject.transform.Rotate(Vector3.up, rotationAmount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerTests : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Pointer DOWN event data: " +  eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log("Pointer UP event data: " + eventData);
    }
}

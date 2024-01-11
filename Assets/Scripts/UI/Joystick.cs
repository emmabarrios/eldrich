using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public static Joystick instance;

    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    public Vector2 Direction { get { return new Vector2(input.x, input.y); } }
    private Vector2 input = Vector2.zero;

    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    //[SerializeField] private bool snapX = false;
    //[SerializeField] private bool snapY = false;
    [SerializeField] float lastTimeTap;
    [SerializeField] float tapThreshold = 0.75f;
    [SerializeField] private RectTransform background = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Texture2D texture = null;
    [SerializeField] private RectTransform handle = null;
    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private float fadeTime = 1f;


    public event EventHandler<OnHandleDragedEventArgs> OnHandleDraged;
    public event EventHandler OnHandleDroped;
    public event EventHandler<OnDoubleTapEventArgs> OnDoubleTap;

    public class OnHandleDragedEventArgs : EventArgs {
        public float magnitude;
    }
    public class OnDoubleTapEventArgs : EventArgs {
        public Vector2 point;
    }


    private void Start() {

        instance = this;

        texture = (Texture2D)image.mainTexture;

        MoveThreshold = moveThreshold;

        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

        background.gameObject.SetActive(false);
    }

    public float HandleRange {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public bool isLocked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        //background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

        //background.gameObject.SetActive(true);
        //StartCoroutine(FadeTo(0, fadeTime));

        //OnDrag(eventData);

        //if (OnArea(eventData)) {
        //    background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

        //    background.gameObject.SetActive(true);
        //    StartCoroutine(FadeTo(0, fadeTime));

        //    OnDrag(eventData);
        //}

        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);

        background.gameObject.SetActive(true);
        StartCoroutine(FadeTo(0, fadeTime));

        OnDrag(eventData);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DoubleTap(eventData);

        if (isLocked == false) {
            background.gameObject.SetActive(false);

            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            OnHandleDroped?.Invoke(this, EventArgs.Empty);

            StartCoroutine(FadeTo(.2f, fadeTime));
        }
    }

    private void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }

        if (magnitude > deadZone) {
            if (magnitude > 1)
                input = normalised;
        } else
            input = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData) {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
        float mag = handle.anchoredPosition.magnitude;
        OnHandleDraged?.Invoke(this, new OnHandleDragedEventArgs {
            magnitude = mag
        });
    }

    private void DoubleTap(PointerEventData eventData) {

        Vector2 center = baseRect.rect.center;
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, handle.position, eventData.pressEventCamera, out localPoint)) {

            float currentTimeClick = eventData.clickTime;

            if (Mathf.Abs(currentTimeClick - lastTimeTap) < tapThreshold) {
                Vector2 distance = localPoint - center;

                OnDoubleTap?.Invoke(this, new OnDoubleTapEventArgs {
                    point = distance
                });
            }
            lastTimeTap = currentTimeClick;
        }

    }

    private Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition) {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint)) {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
    
    private bool OnArea(PointerEventData eventData) {

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) {
            // Convert local point to texture coordinates
            Vector2 uv = new Vector2((localPoint.x + image.rectTransform.pivot.x * image.rectTransform.rect.width) / image.rectTransform.rect.width,
                                     (localPoint.y + image.rectTransform.pivot.y * image.rectTransform.rect.height) / image.rectTransform.rect.height);

            // Convert texture coordinates to pixel coordinates
            int x = Mathf.FloorToInt(uv.x * texture.width);
            int y = Mathf.FloorToInt(uv.y * texture.height);

            // Get the pixel color at the given coordinates
            Color pixelColor = texture.GetPixel(x, y);

            // Check if the pixel color has alpha greater than 0, indicating that it's part of the visible sprite
            if (pixelColor.a > 0) {

                return true;
            }
        }
        return false;
    }

    IEnumerator FadeTo(float aValue, float aTime) {

        float alpha = image.color.a;
        float tempVal = 0f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {

            Color newColor = image.color;
            newColor.a = Mathf.Lerp(alpha, aValue, t);
            image.color = newColor;

            tempVal = newColor.a;

            yield return null;
        }

        // Ensure to completely fade to transparent
        if (image.color.a < 0.1f) {
            Color newColor = image.color;
            newColor.a = 0f;
            image.color = newColor;
        }

    }

    public void PointCursorUp() {
        // Same as OnPointerUp but don't know how to call it without arguments
        background.gameObject.SetActive(false);
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        OnHandleDroped?.Invoke(this, EventArgs.Empty);
        StartCoroutine(FadeTo(.2f, fadeTime));
    }

    public void LockCursor() {
        isLocked = !isLocked;
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Carousel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] int pouchCount;
    [SerializeField] int carouselPosition = 0;
    [SerializeField] float pouchSize = 755f;
    private RectTransform rectTransform;

    public float cooldownTimer = 0f;

    public bool isMoving = false;
    public bool isSwipeInProgress = false;

    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private Vector2 initialTouchPosition;

    [SerializeField] private float interpolationTime;

    public GameObject blocker = null;

    private Coroutine slideCoroutine;

    private void Start() {
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void ModifyImageAlpha(Image image, float alpha) {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
            isSwipeInProgress = true;
            initialTouchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (isSwipeInProgress) {

            if (!isMoving) {


                float swipeDistance = eventData.position.x - initialTouchPosition.x;

                if (Mathf.Abs(swipeDistance) > swipeThreshold) {

                    Vector2 normalDirection = new Vector2(swipeDistance, 0).normalized;
                    float swipeDirection = normalDirection.x;

                    if ((carouselPosition == 0 && swipeDirection > 0)) {
                        isSwipeInProgress = false;
                        isMoving = false;
                        return;
                    }

                    if ((carouselPosition == pouchCount && swipeDirection < 0)) {
                        isSwipeInProgress = false;
                        isMoving = false;
                        return;
                    }

                    isMoving = true;

                    blocker.GetComponent<Image>().raycastTarget = true;

                    slideCoroutine = StartCoroutine(Slide(interpolationTime, swipeDirection));
                    OnSwipe?.Invoke(this, new OnSwipeEventArgs { index = carouselPosition });
                }
            }
        }
        isSwipeInProgress = false;
    }

    private IEnumerator Slide(float interpolationTime, float swipeDirection) {



        Vector2 offset = new Vector2(Mathf.Floor(pouchSize * swipeDirection), 0f);
        Vector2 currentPos = rectTransform.position;

        Vector2 targetPosition = currentPos + offset;
        carouselPosition -= (int)swipeDirection;

        Vector2 velocity = Vector2.zero;

        float t = 0;

        while (Vector2.Distance(rectTransform.position, targetPosition) >20f) {
            t = interpolationTime * Time.deltaTime;

            rectTransform.position = Vector2.SmoothDamp(rectTransform.position, targetPosition, ref velocity, t);
            yield return null;
        }


        rectTransform.position = targetPosition;

        isMoving = false;
        blocker.GetComponent<Image>().raycastTarget = false;
        
    }

    public event EventHandler<OnSwipeEventArgs> OnSwipe;
    public class OnSwipeEventArgs {
        public int index;
    }

    public void CallCoolDownOnUse() {
        StartCoroutine(CoolDownOnUse());
    }

    private IEnumerator CoolDownOnUse() {

        ToggleRaycastTargetsRecursive(this.transform, false);
        yield return new WaitForSeconds(cooldownTimer);
        ToggleRaycastTargetsRecursive(this.transform, true);
    }

    private void ToggleRaycastTargetsRecursive(Transform parent, bool toggleValue) {

        PouchItem[] pouchItems = GetComponentsInChildren<PouchItem>();

        // Access or process the found objects
        foreach (PouchItem pouchItem in pouchItems) {
            Graphic graphic = pouchItem.GetComponent<Graphic>();
            if (graphic != null) {
                graphic.raycastTarget = toggleValue;

                if (toggleValue==false) {
                    ModifyImageAlpha(graphic.transform.GetChild(0).GetComponent<Image>(), 0.4f);
                } else {
                    ModifyImageAlpha(graphic.transform.GetChild(0).GetComponent<Image>(), 1f);
                }
            }
        }
    }

    public void SetPouchCountTotal(int total) {
        this.pouchCount = total - 1;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestureInput : MonoBehaviour
{
    public enum SwipeDir { None, Left, Up, Down, Right, UpLeft, UpRight, DownLeft, DownRight };

    public float minSwipeLength = 5f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;

    [SerializeField] private SwipeDir swipeDirection;

    public SwipeDir SwipeDirection { get { return swipeDirection; } set { swipeDirection = value; } }

    public bool isLocked = false;

    public event EventHandler<SwipeDirectionChangedEventArgs> SwipeDirectionChanged;
    public class SwipeDirectionChangedEventArgs: EventArgs {
        public SwipeDir swipeDirection;
    }


    void Update() {

        if (!GameManager.instance.IsGameOnCombat()) {
            return;
        }

        if (Input.touches.Length > 0) {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began) {

                // Check if the first touch is over any UI element
                if (IsPointerOverUIElement()) {
                    isLocked = true;
                } else {
                    isLocked = false;
                }
                firstPressPos = new Vector2(t.position.x, t.position.y);
                }

            if (isLocked == false) {

                SwipeDir _swipeDirection = new SwipeDir();

                if (t.phase == TouchPhase.Ended) {
                    secondPressPos = new Vector2(t.position.x, t.position.y);
                    currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                    if (currentSwipe.magnitude < minSwipeLength) {
                        _swipeDirection = SwipeDir.None;
                        return;
                    }

                    float swipeAngle = Mathf.Atan2(currentSwipe.y, currentSwipe.x) * Mathf.Rad2Deg;

                    if (swipeAngle < 0) {
                        swipeAngle += 360;
                    }

                    if (swipeAngle > 22.5f && swipeAngle <= 67.5f) {
                        _swipeDirection = SwipeDir.UpRight;
                    } else if (swipeAngle > 67.5f && swipeAngle <= 112.5f) {
                        _swipeDirection = SwipeDir.Up;
                    } else if (swipeAngle > 112.5f && swipeAngle <= 157.5f) {
                        _swipeDirection = SwipeDir.UpLeft;
                    } else if (swipeAngle > 157.5f && swipeAngle <= 202.5f) {
                        _swipeDirection = SwipeDir.Left;
                    } else if (swipeAngle > 202.5f && swipeAngle <= 247.5f) {
                        _swipeDirection = SwipeDir.DownLeft;
                    } else if (swipeAngle > 247.5f && swipeAngle <= 292.5f) {
                        _swipeDirection = SwipeDir.Down;
                    } else if (swipeAngle > 292.5f && swipeAngle <= 337.5f) {
                        _swipeDirection = SwipeDir.DownRight;
                    } else {
                        _swipeDirection = SwipeDir.Right;
                    }
                    CheckIfStateChanged(_swipeDirection);
                }
            }
        }
    }

    public static bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());

    }

    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    private void CheckIfStateChanged(SwipeDir tempDirection) {
        if (SwipeDirection != tempDirection) {
            SwipeDirection = tempDirection;
        }
        SwipeDirectionChanged?.Invoke(this, new SwipeDirectionChangedEventArgs { swipeDirection = SwipeDirection });
    }
}


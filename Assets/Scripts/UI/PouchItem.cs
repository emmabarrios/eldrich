using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PouchItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [Header("Item index")]
    [SerializeField] public int _row = 0;
    [SerializeField] public int _column = 0;

    [Header("New implementation")]
    public QuickItem itemSO;
    public Sprite item_holder_graphic;
    public Image fillImage = null;

    private enum ItemState { Ready, Holstered }

    private ItemState _state = ItemState.Holstered;

    public Sprite fillSprite; // The new sprite to assign to the Image component
    public Color fillSpriteColor;
    public Sprite hintSprite;
    public Color hintSpriteColor;

    [SerializeField] private Toggle toggle;

    // The duration threshold for a long press in seconds
    public float longPressDuration = 3f;

    public float longPressDurationDelay = 1f;

    // Boolean to indicate if a long press has been detected
    public bool isLongPressDetected = false;

    public bool isPressed = false;

    public Vector2 initialPosition;

    public Vector2 initialFillImagePosition;
    [SerializeField] private Vector2 scale;

    public GameObject item_image_gameObject;
    public GameObject item_holder_gameObject;
    public Sprite imageSprite;

    [SerializeField] private GameObject usablePrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private AudioClip drawClip;

    private Coroutine longPressCoroutine;

    public bool canDraw = false;

    Vector2 initialTouchPosition;

    float eventSwipeThreshold = 160f;

    public PlayerPrecense playerPrecense;

    Animator armsAnimator;
    [SerializeField] AudioSource audioSource;

    [SerializeField][Range(0f, 1f)] float volumeScale;

    public void Start() {
        scale = fillImage.GetComponent<RectTransform>().localScale;
        toggle = GameObject.Find("Pouch Toggle").GetComponent<Toggle>();

        usablePrefab = itemSO._usablePrefab;
        projectilePrefab = itemSO._projectilePrefab;
        drawClip = itemSO._drawSound;

    }

    public void OnPointerDown(PointerEventData eventData) {
        fillImage.color = fillSpriteColor;

        initialTouchPosition = eventData.position;
        isPressed = true;
        initialPosition = item_image_gameObject.transform.position;
        initialFillImagePosition = fillImage.transform.position;
        longPressCoroutine = StartCoroutine(LongPressDetectionDelayed(eventData));

        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (_state == ItemState.Ready) {
            float swipeDistance = eventData.position.y - initialTouchPosition.y;

            if (Mathf.Abs(swipeDistance) > eventSwipeThreshold) {
                DrawItem();
                return;
            }
        }

        if (longPressCoroutine != null) {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }

        _state = ItemState.Holstered;

        item_image_gameObject.transform.position = initialPosition;

        isLongPressDetected = false;
        isPressed = false;

        ResetFillAmount();

        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);

    }

    private void ResetFillAmount() {
        if (fillImage != null) {
            fillImage.fillAmount = 0f;
            fillImage.sprite = fillSprite;
            fillImage.transform.position = initialFillImagePosition;
            fillImage.GetComponent<RectTransform>().localScale = scale;
        }
    }

    private void DrawItem() {

        if (usablePrefab != null) {
            GameObject up = Instantiate(usablePrefab, Vector3.zero, Quaternion.identity);

            if (itemSO.type == QuickItem.QuickItemType.Usable) {
                up.GetComponent<IUsable>().Use();
            }
        }


        if (projectilePrefab != null) {
            Projectile projectile = projectilePrefab.GetComponent<Projectile>();
            if (projectile != null) {
                GameObject.FindWithTag("Thrower").GetComponent<Thrower>().LoadThrower(projectile);
            }
        }

        Carousel carousel = transform.parent.transform.parent.transform.parent.GetComponent<Carousel>();
        carousel.cooldownTimer = itemSO._cooldown;
        carousel.CallCoolDownOnUse();
        carousel.GetComponent<AudioSource>().PlayOneShot(drawClip, volumeScale);

        ItemPanelToggle.instance.GetComponent<Toggle>().isOn = false;

        Destroy(this.gameObject);

        CombatInventory.instance.RemoveItem(_row, _column);
    }

    private IEnumerator LongPressDetectionDelayed(PointerEventData eventData) {

        yield return new WaitForSeconds(longPressDurationDelay);

        longPressCoroutine = StartCoroutine(LongPressDetection(eventData));
    }

    private IEnumerator LongPressDetection(PointerEventData eventData) {
        float timer = 0f;

        while (timer < longPressDuration) {

            timer += Time.deltaTime;

            if (fillImage != null) {
                float fillAmount = timer / longPressDuration;
                fillImage.fillAmount = fillAmount;
            }

            Vector2  swipeDistance = new Vector2(eventData.position.x, eventData.position.y) - initialTouchPosition;
            if (Mathf.Abs(swipeDistance.magnitude) > eventSwipeThreshold) {

                ResetFillAmount();
                yield break;
            }

            yield return null;
        }

        _state = ItemState.Ready;

        isLongPressDetected = true;

        fillImage.sprite = hintSprite;
        fillImage.GetComponent<RectTransform>().localScale = fillImage.GetComponent<RectTransform>().localScale / 3f;
        fillImage.color = hintSpriteColor;

        Vector2 offset = new Vector2(0f, 50f);
        Vector2 currentPos = item_image_gameObject.GetComponent<RectTransform>().transform.position;
        Vector2 currentPos2 = fillImage.GetComponent<RectTransform>().transform.position;
        item_image_gameObject.transform.position = currentPos + offset;
        fillImage.transform.position = currentPos2 + offset;

    }

    public bool IsLongPressDetected() {
        return isLongPressDetected;
    }

    public void InitializeQuickItemGraphics() {
        item_image_gameObject.GetComponent<Image>().sprite = itemSO._sprite;
        item_holder_gameObject.GetComponent<Image>().sprite = itemSO._slot_sprite;
    }

    public void SetItemIndex(int r, int c) {
        this._row = r;
        this._column = c;
    }

}

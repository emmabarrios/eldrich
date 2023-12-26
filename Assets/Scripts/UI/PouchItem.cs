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
        //armsAnimator = GameObject.Find("Arms").GetComponent<Animator>();

        usablePrefab = itemSO._usablePrefab;
        projectilePrefab = itemSO._projectilePrefab;
        drawClip = itemSO._drawSound;
    }

    public void OnPointerDown(PointerEventData eventData) {
        // Start the long press detection coroutine
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

        // Stop the long press detection coroutine
        if (longPressCoroutine != null) {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
            //_state = ItemState.Holstered;
        }

        _state = ItemState.Holstered;

        //Reset image to its original position
        item_image_gameObject.transform.position = initialPosition;

        // Reset the long press detection flag
        isLongPressDetected = false;
        isPressed = false;

        // Reset the fill amount of the Image component
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

        // Reproduce corresponding animaton in player visuals

        //if (itemSO.type == QuickItem.QuickItemType.Usable) {
        //    armsAnimator.Play("Use_Item");
           
        //} else {
        //    armsAnimator.Play("Throw_Item");
        //}

        // Instantiate quick item prefab
        if (usablePrefab != null) {
            GameObject up = Instantiate(usablePrefab, Vector3.zero, Quaternion.identity);

            if (itemSO.type == QuickItem.QuickItemType.Usable) {
                up.GetComponent<IUsable>().Use();
            }
        }


        // Load proyectile to thrower if there is a projectile
        if (projectilePrefab != null) {
            Projectile projectile = projectilePrefab.GetComponent<Projectile>();
            if (projectile != null) {
                GameObject.FindWithTag("Thrower").GetComponent<Thrower>().LoadThrower(projectile);
            }
        }

        // Use Item Cooldown
        Carousel carousel = transform.parent.transform.parent.transform.parent.GetComponent<Carousel>();
        carousel.cooldownTimer = itemSO._cooldown;
        carousel.CallCoolDownOnUse();
        carousel.GetComponent<AudioSource>().PlayOneShot(drawClip, volumeScale);

        // Deactivate quick item panel
        ItemPanelToggle.instance.GetComponent<Toggle>().isOn = false;

        // Destroy pouch item
        Destroy(this.gameObject);

        // Destroy item from combat inventory
        CombatInventory.instance.RemoveItem(_row, _column);
    }

    private IEnumerator LongPressDetectionDelayed(PointerEventData eventData) {

        // Wait for 1 second before starting the long press detection
        yield return new WaitForSeconds(longPressDurationDelay);

        longPressCoroutine = StartCoroutine(LongPressDetection(eventData));
    }

    private IEnumerator LongPressDetection(PointerEventData eventData) {
        float timer = 0f;

        while (timer < longPressDuration) {

            timer += Time.deltaTime;

            // Update the fill amount of the Image component
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

        // Long press duration reached, set the flag to true
        isLongPressDetected = true;

        // Reset the image fill when completed
        fillImage.sprite = hintSprite;
        fillImage.GetComponent<RectTransform>().localScale = fillImage.GetComponent<RectTransform>().localScale / 3f;
        fillImage.color = hintSpriteColor;

        //Slightly elevate the image
        Vector2 offset = new Vector2(0f, 50f);
        Vector2 currentPos = item_image_gameObject.GetComponent<RectTransform>().transform.position;
        Vector2 currentPos2 = fillImage.GetComponent<RectTransform>().transform.position;
        item_image_gameObject.transform.position = currentPos + offset;
        fillImage.transform.position = currentPos2 + offset;

    }

    // Method to check if a long press has been detected
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

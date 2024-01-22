using UnityEngine;
using UnityEngine.EventSystems;

public class EventMarker : MonoBehaviour, IPointerClickHandler {

    [SerializeField] private WorldEvent worldEventSO;

    public Vector3 markerLocationId { get; private set; }
    
    [Header("Animation Settings")]
    [SerializeField] private bool animated;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float amplitude = 2.0f;
    [SerializeField] private float frequencey = 0.50f;
    [SerializeField] private float moveRange = 15f;

    private void Start() {
        GameObject instantiatedMarkerObject = Instantiate(worldEventSO._pinObject, this.transform);
        instantiatedMarkerObject.transform.position = this.transform.GetChild(0).transform.position;
        markerLocationId = transform.position;
    }

    private void Update() {
        if (animated) {
            AnimateMarker();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {

        // If the event is a battle, run battle logic, if not, grab loot
        if (worldEventSO._eventType == EventType.battle) {
            GameObject.Find("Canvas").GetComponent<OverworldUIManager>().DisplayStartEventPanel();
            GameManager.instance.LoadEventProperties(this.worldEventSO, this.gameObject);
        } else {
            GeneralInventory.instance.AddItem(worldEventSO._loot[0]);
            Instantiate(worldEventSO._lootPickupFX, this.transform.position, Quaternion.identity);
            DatabaseManager.instance.SaveGame();
            Destroy(this.gameObject);
        }

    }

    private void AnimateMarker() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequencey) * amplitude) + moveRange, transform.position.z);
    }
}

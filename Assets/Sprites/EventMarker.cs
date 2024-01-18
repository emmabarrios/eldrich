
using UnityEngine;
using UnityEngine.EventSystems;

//using Mapbox.Examples;
//using Mapbox.Utils;

public class EventMarker : MonoBehaviour, IPointerClickHandler {

    [SerializeField] private WorldEvent worldEventSO;
    [SerializeField] private GameObject visualInteractionEffect;

    public Vector3 markerLocationId { get; private set; }

    private void Start() {

        GameObject instantiatedMarkerObject = Instantiate(worldEventSO._pinObject, this.transform);
        instantiatedMarkerObject.transform.position = this.transform.GetChild(0).transform.position;
        markerLocationId = transform.position;
    }


    public void OnPointerClick(PointerEventData eventData) {

        // If the event is a battle, run battle logic, if not, grab loot
        if (worldEventSO._eventType == EventType.battle) {
            GameObject.Find("Canvas").GetComponent<OverworldUIManager>().DisplayStartEventPanel();
            GameManager.instance.LoadEventProperties(this.worldEventSO, this.gameObject);
        } else {
            GeneralInventory.instance.AddItem(worldEventSO._loot[0]);
            Instantiate(visualInteractionEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}

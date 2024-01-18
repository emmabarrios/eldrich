
using UnityEngine;
using UnityEngine.EventSystems;

//using Mapbox.Examples;
//using Mapbox.Utils;

public class EventMarker : MonoBehaviour, IPointerClickHandler {

    
    [SerializeField] private WorldEvent eventScriptableObject;

    [SerializeField] private GameObject markerObject;

    public Vector3 markerLocationId { get; private set; }

    [SerializeField]
    private EventType eventType;

    //LocationStatus playerLocation;
    //public Vector2d eventPos;

    OverworldUIManager overworldUIManager;

    private void Start() {
        //menuUiManager = GameObject.Find("Canvas").GetComponent<MenuUIManager>();
        //overworldUIManager = GameObject.Find("Canvas").GetComponent<OverworldUIManager>();
        markerObject = eventScriptableObject._markerObject;

        GameObject instantiatedMarkerObject = Instantiate(markerObject, this.transform);
        instantiatedMarkerObject.transform.position = this.transform.GetChild(0).transform.position;

        markerLocationId = transform.position;
        //GameManager.instance.OnCombatEventFinished += DestroyAfterCombatEvent;
    }


    public void OnPointerClick(PointerEventData eventData) {

        if (eventType == EventType.battle) {
            GameObject.Find("Canvas").GetComponent<OverworldUIManager>().DisplayStartEventPanel();
            GameManager.instance.LoadEventProperties(this.eventScriptableObject, this.gameObject);
        } else {
            GeneralInventory.instance.AddItem(eventScriptableObject._itemList[0]);
            Destroy(this.gameObject);
        }

    }
}

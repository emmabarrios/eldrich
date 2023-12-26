
using UnityEngine;
using UnityEngine.EventSystems;

//using Mapbox.Examples;
//using Mapbox.Utils;

public class EventMarker : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float amplitude = 2.0f;
    [SerializeField] private float frequencey = 0.50f;
    [SerializeField] private float moveRange = 15f;

    [SerializeField] private WorldEvent eventScriptableObject;

    [SerializeField] private GameObject markerObject;

    public Vector3 markerLocationId { get; private set; }


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

    void Update()
    {
        //FloatAndRotatePointer();
    }

    private void FloatAndRotatePointer() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequencey) * amplitude) + moveRange, transform.position.z);
    }

    //private void OnMouseUp() {
    //    if (!EventSystem.current.IsPointerOverGameObject()){
    //        if (GameObject.Find("Canvas").transform.GetChild(0).gameObject.activeSelf == false) {
    //            GameManager.instance.LoadEventProperties(this.eventScriptableObject, this.gameObject);
    //        }
    //        GameObject.Find("Canvas").GetComponent<OverworldUIManager>().DisplayStartEventPanel();
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData) {
        if (GameObject.Find("Canvas").transform.GetChild(0).gameObject.activeSelf == false) {
            GameManager.instance.LoadEventProperties(this.eventScriptableObject, this.gameObject);
        }
        GameObject.Find("Canvas").GetComponent<OverworldUIManager>().DisplayStartEventPanel();
    }
}

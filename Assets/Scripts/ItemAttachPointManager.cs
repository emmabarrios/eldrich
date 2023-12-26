using System.Collections;
using UnityEngine;

public class ItemAttachPointManager : MonoBehaviour {

    [Header("Attach Points")]
    [SerializeField] private Vector3 positionOffset_A;
    [SerializeField] private Vector3 positionOffset_B;
    [SerializeField] private Quaternion rotationOffset_A;
    [SerializeField] private Quaternion rotationOffset_B;
    private Transform attachPointA;
    private Transform attachPointB;

    public GameObject child;

    [Header("Timer Settings")]
    [SerializeField] private float attackToHandTimer;
    [SerializeField] private float lifeTime;
    private bool isTiming;

    private void Update() {
        if (isTiming) {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0.1f) {
                lifeTime = 0;
                isTiming = false;
                Destroy(gameObject);
            }
        }
    }
    private void Start() {
        isTiming = true;
        StartCoroutine(InitializeProperties());
        AttachToPoint(attachPointA);
        this.transform.localPosition = positionOffset_A;
        StartCoroutine(AttachToHand());
    }


    public void AttachToPoint(Transform point) {
        this.transform.SetParent(point);
    }

    private IEnumerator AttachToHand() {
        yield return new WaitForSeconds(attackToHandTimer);
        if (child != null) {
            Animator childAnmator = child.GetComponent<Animator>();
            childAnmator.SetTrigger("isAttached");
        }
        AttachToPoint(attachPointB);
       
        this.transform.localRotation = Quaternion.identity; 
        this.transform.localPosition = Vector3.zero;

        this.transform.rotation *= rotationOffset_B;
        this.transform.position += positionOffset_B;
    }

    private IEnumerator InitializeProperties() {
        yield return attachPointA = GameObject.Find("Player Attach Point").GetComponent<Transform>();
        //yield return attachPointB = GameObject.Find("Item Anchor").GetComponent<Transform>();
    }
}

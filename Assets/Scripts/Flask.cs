using UnityEngine;

public class Flask : MonoBehaviour
{
    
    [SerializeField] private Vector3 flaskOffset;
    [SerializeField] private GameObject flaskSeal;
    [SerializeField] private GameObject flaskEmptyContainer;
    [SerializeField] private GameObject SealPrefab;

    [Header("Seal throwing options")]
    public float throwForce;
    public float throwUpwardForce;
    [SerializeField] private Vector3 offset;

    [SerializeField] private PlayerPrecense playerPrecense;

    [SerializeField] private Thrower thrower;

    private void Start() {
        thrower = GameObject.Find("Seal Thrower").GetComponent<Thrower>();
    }

    public void Open() {
        flaskSeal.SetActive(false);
    }

    public void Drop() {
        Transform parentTransform = GameObject.Find("Player Attach Point").GetComponent<Transform>();
        GameObject _emptyBottle = Instantiate(flaskEmptyContainer, parentTransform.position + flaskOffset, parentTransform.rotation);
        _emptyBottle.transform.rotation = parentTransform.rotation;

        Destroy(GetComponentInParent<HealthPotion>().gameObject);
    }
}

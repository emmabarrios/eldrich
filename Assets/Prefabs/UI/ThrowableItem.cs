using System.Collections;
using UnityEngine;

public class ThrowableItem : MonoBehaviour, IUsable {
    [SerializeField] private GameObject projectile;
    public float throwForce;
    public float throwUpwardForce;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Thrower thrower;
    public float timer;
    private bool isTiming;

    //[SerializeField] private float rotationForce = 5f;

    private void Start() {
        thrower = GameObject.Find("Item Thrower").GetComponent<Thrower>();
    }

    public void Use() {
        isTiming = true;
        GameObject.Find("Player Visual").GetComponent<Animator>().Play("Throw_Item");
        //StartCoroutine(DelayedUse());
    }

    private void Update() {
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0; ;
                //thrower.Throw(projectile, throwForce, throwUpwardForce, rotationForce, offset);

                isTiming = false;
                Destroy(gameObject);
            }
        }
    }

}


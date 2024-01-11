using System;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    private bool isTiming = false;

    [SerializeField] private Transform parentTarget;

    [SerializeField] private float areaSpawnTime = 0f;

    [SerializeField] private float damage;

    //public event EventHandler OnHitDeflected;


    void Start()
    {
        _collider = GetComponent<Collider>();
        parentTarget = GetComponent<Transform>().root;

    }

    void Update()
    {
        if (isTiming) {
            areaSpawnTime -= Time.deltaTime;
            if (areaSpawnTime < 0.1f) {
                _collider.enabled = false;
                isTiming = false;
                areaSpawnTime = 0;
                //areaType = AreaType.None;
            }
        }
    }

    public void ActivateHitArea(float damage, float window) {
        //areaType = AreaType.Attack;
        this.damage = damage;
        areaSpawnTime = window;
        isTiming = true;
        _collider.enabled = true;
    }

    public void DeactivateHitArea() {
        isTiming = false;
        _collider.enabled = false;
        areaSpawnTime = 0f;
    }

    private void OnTriggerEnter(Collider other) {

        if (!other.gameObject.CompareTag(parentTarget.tag)) {

            Controller controller = other.gameObject.GetComponent<Controller>();
            parentTarget.GetComponent<Animator>().SetTrigger("staggered");

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                DeactivateHitArea();
                damageable.TakeDamage(this.damage);
            }
        }
    }

}

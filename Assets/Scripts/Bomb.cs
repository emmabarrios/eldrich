using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Bomb : Projectile
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionDamage;
    [SerializeField] private GameObject explosionFX;
    [Range(0f, 10f)] public float explosionTimer;
    private bool isTiming;

    [Header("Colliders In Range")]
    [SerializeField] private List<Collider> inRangeColliders = new List<Collider>();

    private new void Start() {
        base.Start();
        isTiming = true;
    }

    private void Update() {

        // Bomb timer
        if (isTiming) {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer < 0.1f) {
                explosionTimer = 0;
                isTiming = false;
                Explode();
            }
        }
    }

    private void Explode() {
        DealDamageOnRadius();
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null) {
            if (!inRangeColliders.Contains(other)) {
                inRangeColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (inRangeColliders.Contains(other)) {
            inRangeColliders.Remove(other);
        }
    }

    private void DealDamageOnRadius() {
        foreach (Collider collider in inRangeColliders) {
            IDamageable damageable = collider.GetComponentInParent<IDamageable>();

            if (damageable != null) {
                damageable.TakeDamage(explosionDamage);
            }
        }
    }

    //private void OnTriggerEnter(Collider other) {
    //    BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();
    //    if (bodyPart!=null) {
    //        if(!inRangeColliders.Contains(other)) {
    //            inRangeColliders.Add(other);
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other) {
    //    if (inRangeColliders.Contains(other)) {
    //        inRangeColliders.Remove(other);
    //    }
    //}

    //private void DealDamageOnRadius() {
    //    foreach (Collider collider in inRangeColliders) {
    //        BodyPart bodyPart = collider.gameObject.GetComponent<BodyPart>();

    //        if (bodyPart != null) {

    //            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

    //            if (damageable != null) {
    //                damageable.TakeDamage(explosionDamage);
    //            }
    //        }
    //    }
    //}

}

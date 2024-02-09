using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRadius : MonoBehaviour
{
    private float explosionDamage;
    public float ExplosionDamage { get { return explosionDamage; } set {  explosionDamage = value; } }

    private void OnTriggerEnter(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();
            if (damageable != null) {
                if (other.gameObject.name != "Player") {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    damageable.TakeDamage(explosionDamage);
                    Destroy(gameObject);
                }
            }
        }
    }
}

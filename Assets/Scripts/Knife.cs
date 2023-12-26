using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Projectile
{
    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Enemy") {
            DealDamageOnImpact(other);
        }

        if ((other.tag == "Enemy" || other.tag == "Terrain") && destroyOnImpact == true) {
            if (impactFX != null) {
                Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                Instantiate(impactFX, contactPoint, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    protected void DealDamageOnImpact(Collider other) {

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null) {
            damageable.TakeDamage(impactDamage);
        }
    }
}

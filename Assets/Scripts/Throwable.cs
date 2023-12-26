using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public float damage;
    public GameObject damageFX;

    protected void DealDamage(Collider other) {
        BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

        if (bodyPart != null) {
            IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

            if (damageable != null) {
                if (other.gameObject.name != "Player") {
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    Instantiate(damageFX, contactPoint, Quaternion.identity);

                    float finalDamage;

                    switch (bodyPart.tag) {
                        case "Head":
                            finalDamage = damage + 100;
                            break;
                        case "Chest":
                            finalDamage = damage + 30;
                            break;
                        default:
                            finalDamage = damage;
                            break;
                    }

                    damageable.TakeDamage(finalDamage);
                    Destroy(gameObject);
                }
            }
        } else {
            Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            Instantiate(damageFX, contactPoint, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}

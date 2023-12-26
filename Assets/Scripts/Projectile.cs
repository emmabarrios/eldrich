using UnityEngine;

public class Projectile : MonoBehaviour {

    protected AudioSource audioSource;

    [Header("Throw Settings")]
    [SerializeField] protected float throwForce;
    [SerializeField] protected float throwUpwardForce;
    [SerializeField] protected float rotationForce = 5f;
    [SerializeField] protected float spawnDelay = 1f;
    [SerializeField] protected float useCooldown = 1f;
    [SerializeField] protected Vector3 offset;

    public float ThrowForce { get { return throwForce; } }
    public float ThrowUpwardForce { get { return throwUpwardForce; } }
    public float RotationForce { get { return rotationForce; } }
    public float SpawnDelay { get { return spawnDelay; } }
    public float UseCooldown { get { return useCooldown; } }
    public Vector3 Offset { get { return offset; } }

    [Header("Impact Settigs")]
    [SerializeField] protected float impactDamage;
    [SerializeField] protected bool destroyOnImpact = false;
    [SerializeField] protected GameObject impactFX;

    [Header("FX Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    float volumeScale;
    [SerializeField] protected AudioClip impactSoundFX;
    [SerializeField] protected AudioClip throwSoundFX;
    [SerializeField] protected AudioClip loopSoundFX;


    protected void Start() {
        audioSource = GetComponent<AudioSource>();

        if (loopSoundFX!=null) {
        }
        if (throwSoundFX != null) {
            audioSource.PlayOneShot(throwSoundFX, volumeScale);
        }
    }


    protected void OnCollisionEnter(Collision collision) {
        if (impactSoundFX!=null) {
            audioSource.PlayOneShot(impactSoundFX, volumeScale);
        }
    }
    //private void OnTriggerEnter(Collider other) {

    //    if (impactFX != null) {
    //        Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
    //        Instantiate(impactFX, contactPoint, Quaternion.identity);
    //    }
    //    Debug.Log(other.name);

    //    DealDamageOnImpact(other);

    //    if (destroyOnImpact == true) {
    //        Destroy(gameObject);
    //    }
    //}

    //protected void DealDamageOnImpact(Collider other) {
    //    BodyPart bodyPart = other.gameObject.GetComponent<BodyPart>();

    //    if (bodyPart != null) {
    //        IDamageable damageable = bodyPart.GetComponentInParent<IDamageable>();

    //        if (damageable != null) {
    //            if (other.gameObject.name != "Player") {

    //                float finalDamage;

    //                // Localized damage
    //                switch (bodyPart.tag) {
    //                    case "Head":
    //                        finalDamage = impactDamage + 100;
    //                        break;
    //                    case "Chest":
    //                        finalDamage = impactDamage + 30;
    //                        break;
    //                    default:
    //                        finalDamage = impactDamage;
    //                        break;
    //                }

    //                damageable.TakeDamage(finalDamage);

    //            }
    //        }
    //    }
    //}

}
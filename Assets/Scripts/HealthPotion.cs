using System.Collections;
using UnityEngine;

public class HealthPotion : MonoBehaviour, IUsable
{
    public float healthRecoveryPoints;

    public float delayTime;
    public Player user;

    public void Use() {
        StartCoroutine(DelayedUse());
    }

    public void Start() {
        user = GameObject.Find("Player").GetComponent<Player>();
    }

    private IEnumerator DelayedUse() {
        yield return new WaitForSeconds(delayTime);

        if (user != null) {
            user.RecoverHealth(healthRecoveryPoints);
        }
    }
}

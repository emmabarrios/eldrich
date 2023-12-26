using System.Collections;
using UnityEngine;

public class ConsumableItem : MonoBehaviour, IUsable
{
    [Header("Attribute modifiers")]
    public float health;
    public float stamina;
    public float equipLoad;
    public float poise;
    public float poisonDefense;
    public float fireDefense;
    public float lightningDefense;
    public float magicDefense;
    public float frostbiteDefense;
    public float strikeDefense;
    public float slashDefense;
    public float thrustDefense;

    public float delayTime;
    public Player user;

    public void Use() {
        GameObject.Find("Player Visual").GetComponent<Animator>().Play("Use_Item");
        StartCoroutine(DelayedUse());
    }

    public void Start() {
        user = GameObject.Find("Player").GetComponent<Player>();
    }

    private IEnumerator DelayedUse() {
        yield return new WaitForSeconds(delayTime);

        if (user != null) {
            user.RecoverHealth(health);
        }
    }
}

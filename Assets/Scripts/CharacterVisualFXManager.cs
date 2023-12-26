using UnityEngine;

public class CharacterVisualFXManager : MonoBehaviour
{
    [Header("Damage FX")]
    public GameObject bloodSplatterFX;

    public void PlayDamageEffect(Vector3 damageEffectLocation) {
        GameObject blood = Instantiate(bloodSplatterFX, damageEffectLocation, Quaternion.identity);
    }
}

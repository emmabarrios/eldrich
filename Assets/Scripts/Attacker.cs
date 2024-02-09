using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    CharacterSoundFXManager characterSoundFXManager;

    [Header("Combo settings")]
    [SerializeField] private float attackBonus_0;
    [SerializeField] private float attackBonus_1;
    [SerializeField] private float attackBonus_2;
    [SerializeField] private float attackBonus_3;
    [SerializeField] private float current_bonus;
    [SerializeField] private float comboTimeLimit = 3f;
    [SerializeField] private float currentComboTime = 0f;

    [Header("Attack settings")]
    [SerializeField] private float hit_distance = 1f;
    [SerializeField] private float ray_delay = .2f;
    [SerializeField] private float damage;



    public void UpdateComboTimerLimit(float weaponCooldown) {
        comboTimeLimit += weaponCooldown;
    }

    public void UpdateDamage(float damage) {
        this.damage = damage;
    }

    public Action<string> OnAttackLanded;

    private Dictionary<string, List<string>> comboDict = new Dictionary<string, List<string>>
    {
        { "RAGE", new List<string> { "Swing_Left", "Swing_Left", "Swing_Right", "Swing_Down" } },
        { "HEAVY SWING", new List<string> { "Swing_Left", "Swing_Stab", "Swing_Down" } },
        { "FAST BLADE", new List<string> { "Swing_Left", "Swing_Right" } }
    };

    private List<string> currentInput = new List<string>();

    private string lastCombo;

    private void Start() {
        characterSoundFXManager = transform.root.GetComponentInChildren<CharacterSoundFXManager>();
    }

    void Update()
    {
        if (currentComboTime > 0f) {
            currentComboTime -= Time.deltaTime;
        } else {
            currentComboTime = 0f;
        }
    }

    private string EvaluateCombo(string attackDirection) {
        if (currentComboTime <= 0f) {
            currentInput.Clear();
        }

        currentComboTime = comboTimeLimit;

        currentInput.Add(attackDirection);

        foreach (var combo in comboDict) {
            if (MatchCombo(currentInput, combo.Value)) {
                currentInput.Clear();
                lastCombo = combo.Key + "!!";
                return combo.Key; // Return the name of the triggered combo
            }
        }
        lastCombo = null;
        return null; // Return null if no combo is triggered
    }

    public void Attack(string attackDirection) {

        string evaluatedCombo = EvaluateCombo(attackDirection);
        characterSoundFXManager.PlayRandomSwingSoundFX();

        switch (evaluatedCombo) {
            case "RAGE":
                current_bonus = attackBonus_1;
                break;

            case "HEAVY SWING":
                current_bonus = attackBonus_2;
                break;

            case "FAST BLADE":
                current_bonus = attackBonus_3;
                break;

            default:
                current_bonus = attackBonus_0;
                break;
        }

        StartCoroutine(ShootRayCoroutine(ray_delay));
    }

    private bool MatchCombo(List<string> input, List<string> combo) {
        if (input.Count != combo.Count) {
            return false;
        }

        for (int i = 0; i < input.Count; i++) {
            if (input[i] != combo[i]) {
                return false;
            }
        }

        return true;
    }

    private void ShootRay() {
        Vector3 rayDirection = transform.forward; // Assuming the attacker is facing forward

        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, hit_distance)) {
            if (hitInfo.collider.CompareTag("Enemy")) {
                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
                if (damageable != null) {
                    damageable.TakeDamage(damage * current_bonus); // Call the TakeDamage method on the enemy
                }

                // Fire event
                OnAttackLanded?.Invoke($"{lastCombo} {damage * current_bonus}");
                hitInfo.collider.GetComponent<CharacterSoundFXManager>().PlayDamageSoundFX();
            }
        }
        // Draw a debug ray to visualize the raycast
        Debug.DrawRay(transform.position, rayDirection * hit_distance, Color.red, 0.1f);

    }

    private IEnumerator ShootRayCoroutine(float time) {
        yield return new WaitForSeconds(time);
        ShootRay();
    }

}

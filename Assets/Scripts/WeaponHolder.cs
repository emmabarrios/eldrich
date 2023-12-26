using System;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Controller controller;

    private float timer = 0f;
    private float timerDuration = 0.5f;

    private void Awake() {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<Controller>();
    }

    void Start()
    {
        controller.OnAttack += PlayAttackAnimation;
        controller.OnDash += PlayDashAnimation;
        
    }

    private void Update() {

        if (controller.IsWalking) {
            timer += Time.deltaTime;

            if (timer >= timerDuration) {
                transform.root.GetComponentInChildren<CharacterSoundFXManager>().PlayRandomFootstepSoundFX();
                timer = 0f;
            }
        } else {
            // Reset the timer if IsWalking is false
            timer = 0f;
        }
        animator.SetBool("isWalking", controller.IsWalking);
    }

    private void PlayAttackAnimation(string dir) {
        switch (dir) {

            case "Swing_Left":
                animator.Play("weapon_holder_attack_left", -1, 0);
                break;
            case "Swing_Stab":
                animator.Play("weapon_holder_attack_up", -1, 0);
                break;
            case "Swing_Right":
                animator.Play("weapon_holder_attack_right", -1, 0);
                break;
            case "Swing_Down":
                animator.Play("weapon_holder_attack_down", -1, 0);
                break;
            default:
                break;
        }
    }

    private void PlayDashAnimation(int dir) {
        animator.Play("weapon_holder_dash", -1, 0);
    }
}

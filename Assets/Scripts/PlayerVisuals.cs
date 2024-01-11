using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [Header("Weapon Anchors")]
    //[SerializeField] private Transform anchorPointL = null;
    //[SerializeField] private Transform anchorPointR = null;

    [SerializeField] private PlayerPrecense animator;

    [Header("Timer Settings")]
    [SerializeField] private float timer;
    [SerializeField] private bool isTiming = false;

    public event EventHandler OnWeaponHit;

    private void Start() {
        animator = GetComponentInParent<PlayerPrecense>();

        //animator.OnUsingItem += RunStatusTimer_OnUsingItem;


        //anchorPointL = GameObject.Find("Player Weapon Anchor Point R").GetComponent<Transform>();
        //anchorPointL = null;
        //anchorPointR = GameObject.Find("Player Weapon Anchor Point R").GetComponent<Transform>();
        //anchorPointR = null;
    }

    private void Update() {
        // Toggle Visuals Timer 
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;
                ToggleWeaponsVisuals(true);
                isTiming = false;
            }
        }
    }

    public void ToggleWeaponsVisuals(bool toggle) {
        //if (toggle == false) {
        //    anchorPointL.GetChild(0).gameObject.SetActive(false);
        //    anchorPointR.GetChild(0).gameObject.SetActive(false);
        //} else {
        //    anchorPointL.GetChild(0).gameObject.SetActive(true);
        //    anchorPointR.GetChild(0).gameObject.SetActive(true);
        //}
    }

    //public void RunStatusTimer_OnUsingItem(object sender, PlayerAnimator.OnUsingItemEventArgs e) {
    //    ToggleWeaponsVisuals(false);
    //    isTiming = true;
    //    timer = e.animLength;
    //}

    public void InvokeOnWeaponHit() {
        OnWeaponHit?.Invoke(this,  EventArgs.Empty);
    }

}

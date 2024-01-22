using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Character, IDamageable {

    [SerializeField] private float currentHealth;
    [SerializeField] float maxHealth;

    [SerializeField] Animator visualAnimator;

    [SerializeField] private Image lifebarImage = null;
    [SerializeField] float walkSpeed;

    CharacterController controller;

    
    [SerializeField] private bool isDefeated = false;
    [SerializeField] private bool isOnWalkingAnimation;

    public float attemptHitTime;
    public bool isWindowAttackOpen;

   

    public float attackTimer;
    public float attackTime = 1;

    [SerializeField] private CharacterSoundFXManager characterSoundFXManager;


    public Action<float> OnDamageTaken;

    [SerializeField] Player player;

    private Transform playerTransform;


    [Header("Combat state")]
    [SerializeField] private bool hasCombatFinished = false;

    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] public float translationSpeed = 5f;

    [Header("Chace settings")]
    [SerializeField] private float delayTransitionLowerLimit;
    [SerializeField] private float delayTransitionUpperLimit;
    [SerializeField] private bool isWalking = true;
    [SerializeField] private bool hasTransitionToChaseStarted = false;

    [Header("Attack settings")]
    [SerializeField] private bool hasAttacked = false;
    [SerializeField] private bool hasAttackStarted = false;
    [SerializeField] private float attackCooldown;
    [SerializeField] float attackDelay;
    [SerializeField] float attackSoundDelayTime;


    [Header("Raycast settings")] 
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float raycastOffset = 0.5f;
    [SerializeField] private float yOffset;

    [Header("Hit settings")]
    [SerializeField] private float recoveryTime;
    [SerializeField] private float recoveryTimer;
    [SerializeField] private bool attackInterrupted = false;


    [Header("State settings")]
    public EnemyState currentState = EnemyState.Idle;


    private void Awake() {
        // Target lock the enemy on the player
        player = GameObject.Find("Player").GetComponent<Player>();
        player.GetComponent<Controller>().SetLockTarget(this.transform);
        playerTransform = player.GetComponent<Transform>();
    }

    private void Start() {
        controller = GetComponent<CharacterController>();
        visualAnimator = GetComponent<Animator>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        currentHealth = Health;
        maxHealth = Health;
    }

    void Update() {

        visualAnimator.SetBool("isWalking", isWalking);

        if (Health <= 0) {
            if (!isDefeated) {
                isDefeated = true;
                currentState = EnemyState.Dead;
                GameManager.instance.EndCombatSequence(isDefeated);
            }
        }

        // Enemy is not defeated but prevent creation of another variable
        if (player.Health <= 0) {
            if (hasCombatFinished == false) {
                hasCombatFinished = true;
                currentState = EnemyState.Neutral;
                GameManager.instance.EndCombatSequence(false);
            } 
        }


        switch (currentState) {
            case EnemyState.Idle:
                IdleState();
                break;

            case EnemyState.Chase:
                ChaseState();
                break;

            case EnemyState.Attack:
                AttackState();
                break;
            
            case EnemyState.Dead:
                DeadState();
                break;
            
            case EnemyState.Neutral:
                NeutralState();
                break;
            
            case EnemyState.Hit:
                HitState();
                break;
        }
    }

    void HitState() {

        recoveryTimer -= Time.deltaTime;
        if (recoveryTimer <= 0f) {
            recoveryTimer = recoveryTime;
            currentState = (UnityEngine.Random.Range(0, 2) == 0) ? EnemyState.Idle : EnemyState.Chase;
        }
    }
    void DeadState() {
        isWalking = false;
    }

    void NeutralState() {
        isWalking = false;
    }

    void IdleState() {

        isWalking = false;

        RotateTowardsTarget(rotationSpeed, Time.deltaTime);

        //Get the current state information
        AnimatorStateInfo stateInfo = visualAnimator.GetCurrentAnimatorStateInfo(0);

        // Check the name of the current animation
        bool isOnIntroAnimation = stateInfo.IsName("Intro");

        if (!isOnIntroAnimation) {
            // Rotate towards the player while deciding the time to start chasing
            RotateTowardsTarget(rotationSpeed, Time.deltaTime);

            // If player is on front, change to attack state
            if (IsPlayerInFront() && hasAttackStarted == false) {
                currentState = EnemyState.Attack;
            } else if (hasTransitionToChaseStarted == false) {
                hasTransitionToChaseStarted = true;
                StartCoroutine(TransitionToChaseCoroutine(delayTransitionLowerLimit, delayTransitionUpperLimit));
            } 
        }

        if (player.Health <= 0) {
            currentState = EnemyState.Neutral;
        }
    }

    void ChaseState() {
        // Implement behavior for the Chase state

        RotateTowardsTarget(rotationSpeed, Time.deltaTime);
        //TranslateTowardsTarget();

        if (!IsPlayerInFront()) {
            TranslateTowardsTarget();
        }
        
        if(IsPlayerInFront() && hasAttackStarted == false) {
            currentState = EnemyState.Attack;
        }

        if (player.Health <= 0) {
            currentState = EnemyState.Neutral;
        }
    }

    void AttackState() {
        isWalking = false;

        if (hasAttackStarted == false) {
            hasAttackStarted = true;
            StartCoroutine(PlaySwingSoundDelayed(attackSoundDelayTime));
            StartCoroutine(AttackCoroutine());
        }
       
    }

    public void TakeDamage(float damage) {
        isWalking = false;

        //animator.Play("Skeleton@Damage01");
        //characterSoundFXManager.PlayDamageSoundFX();
        Health -= damage;
        lifebarImage.fillAmount = Health / maxHealth;
        OnDamageTaken?.Invoke(Health);

       

        if (Health <= 0) {
            visualAnimator.Play("dead", -1, 0);
        } else{
            visualAnimator.Play("hit", -1, 0);
            currentState = EnemyState.Hit;
            recoveryTimer = recoveryTime;
            attackInterrupted = true;
        }
    }

    private bool IsPlayerInFront() {
        Color rayColor = Color.blue;
        Vector3 rayStart = transform.position + transform.forward * raycastOffset + new Vector3(0f, yOffset, 0f);

        // Perform the raycast
        RaycastHit hit;

        if (Physics.Raycast(rayStart, transform.forward, out hit, attackRange, targetLayer)) {
            Player player = hit.collider.GetComponent<Player>();
            if (player != null && player.Health > 0) {
                return true;
            }
        }
        Debug.DrawRay(rayStart, transform.forward * attackRange, rayColor);
        return false;
    }

    private void RotateTowardsTarget(float followRotationSpeed, float timeDelta) {
        Vector3 directionToTarget = playerTransform.position - transform.position;
        directionToTarget.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
    }

    private void TranslateTowardsTarget() {
        Vector3 moveDirection = transform.forward;
        controller.Move(moveDirection * walkSpeed * Time.deltaTime);
        isWalking = true;
    }

    IEnumerator AttackCoroutine() {
        visualAnimator.Play("attack", -1, 0);

        if (CheckAttackInterrupted())
            yield break;

        yield return new WaitForSeconds(attackDelay);

        if (CheckAttackInterrupted())
            yield break;

        // Print "Attack" only once
        AttackIfNotAttacked();

        if (CheckAttackInterrupted())
            yield break;

        // Wait for attack cooldown
        yield return new WaitForSeconds(attackCooldown);

        if (CheckAttackInterrupted())
            yield break;

        // Reset the flag and transition back to Chase state
        ResetAttackFlagsAndTransition();
    }

    bool CheckAttackInterrupted() {
        if (attackInterrupted) {
            ResetAttackFlags();
            return true;
        }
        return false;
    }

    void AttackIfNotAttacked() {
        if (!hasAttacked) {
            AttackPlayer();
            hasAttacked = true;
        }
    }

    void ResetAttackFlagsAndTransition() {
        hasAttacked = false;

        if (!IsPlayerInFront()) {
            currentState = (UnityEngine.Random.Range(0, 2) == 0) ? EnemyState.Idle : EnemyState.Chase;
        }

        hasAttackStarted = false;
    }

    void ResetAttackFlags() {
        hasAttackStarted = false;
        hasAttacked = false;
        attackInterrupted = false;
    }

    IEnumerator TransitionToChaseCoroutine(float a, float b) {
        float randomDelay = UnityEngine.Random.Range(a, b);
        yield return new WaitForSeconds(randomDelay);

        // Transition to Chase state
        currentState = EnemyState.Chase;
        hasTransitionToChaseStarted = false;
    }

    private void AttackPlayer() {
        Color rayColor = Color.red;
        Vector3 rayStart = transform.position + transform.forward * raycastOffset + new Vector3(0f, yOffset, 0f);
        RaycastHit hit;
        if (Physics.Raycast(rayStart, transform.forward, out hit, attackRange, targetLayer)) {
            Player player = hit.collider.GetComponent<Player>();
            if (player != null) {
                player.TakeDamage(Damage);
                player.GetComponentInChildren<CharacterSoundFXManager>().PlayDamageSoundFX();
            }
        }
        Debug.DrawRay(rayStart, transform.forward * attackRange, rayColor);
    }

    private IEnumerator PlaySwingSoundDelayed(float time) {
        yield return new WaitForSeconds(time);
        characterSoundFXManager.PlayRandomSwingSoundFX();
    }
}
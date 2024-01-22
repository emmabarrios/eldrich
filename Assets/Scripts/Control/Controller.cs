using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour {

    private Player player;
    private CharacterController characterController;
    private Attacker attacker;

    private bool hasCombatStarted = false;

    [Header("Orbital Settings")]
    [SerializeField] private Transform target = null;
    private float lookRotationSpeed = 4f;

    [Header("Movement Settings")]
    //[SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float deaccelerationTime = 2f;
    [SerializeField] private float speedLimitMultiplier = 1f;
    //[SerializeField] private float turnSpeed = 90f;
    [SerializeField] private bool isWalking;

    private float currentSpeed;
    private Vector3 last_movement;

    [Header("Knock Back Settings")]
    [SerializeField] private float knockBackDistance;
    [SerializeField] private float knockBackTime;
    [SerializeField] private float knockBackTimer;
    [SerializeField] private bool isKnockBacked = false;
    [SerializeField] private bool isHitBlocked = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashMovementSpeed;
    //[SerializeField] private float dirMultiplier = 1f;
    [SerializeField] private float lookRotationSpeedMultiplier = 4f;
    [SerializeField] private bool dashPerformed = false;

    [Header("Input")]
    public Joystick joystick = null;
    public GestureInput gestureInput;
    public ItemPanelToggle quickItemToggle = null;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackTimer;

    [SerializeField] private bool attackPerformed = false;
    [SerializeField] private bool isUsingItem = false;

    // Events
    public Action OnBlocking;
    public Action OnReleaseBlock;
    public Action OnParry;
    public Action<int> OnDash;
    public Action<string> OnAttack;
    public Action<float> OnAttackCooldown;


    public bool IsWalking { get { return isWalking; } set { isWalking = value; } }
    public bool DashPerformed { get { return dashPerformed; } set { dashPerformed = value; } }
    public bool AttackPerformed { get { return attackPerformed; } set { attackPerformed = value; } }
    public bool IsHitBlocked { get { return isHitBlocked; } set { isHitBlocked = value; } }
    public float CurrentSpeed { get { return currentSpeed; } }
    public bool IsUsingItem { get { return isUsingItem; } set { isUsingItem = value; } }
    public bool IsKnockBacked { get { return isKnockBacked; } set { isKnockBacked = value; } }

    private void Start() {

        player = GetComponent<Player>();
        characterController = GetComponent<CharacterController>();

        // Player Input
        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        gestureInput = GameObject.Find("Gesture Input").GetComponent<GestureInput>();
        quickItemToggle = GameObject.Find("Pouch Toggle").GetComponent<ItemPanelToggle>();

        // Input Event Subscribers
        gestureInput.SwipeDirectionChanged += HandleGestureInput;
        joystick.OnDoubleTap += Dash;

        // Initial Values
        currentSpeed = 0f;

        // Add reference to the attacker component
        attacker = GetComponentInChildren<Attacker>();

    }

    private void Update() {

        if (!GameManager.instance.IsGameOnCombat()) {
            if (IsWalking == true) {
                IsWalking = false;
            }
            return;
        }

        if (hasCombatStarted == false) {
            hasCombatStarted = true;
        }

        // Movement
        Vector2 inputMovement = joystick.Direction;
        if (!DashPerformed && !IsKnockBacked) {
            Orbitate(inputMovement);
        }

        // Evaluate attack performed
        if (AttackPerformed) {
            attackTimer -= Time.deltaTime;

            float fillAmount = Mathf.Clamp01(1f - (attackTimer / attackCooldown));
            OnAttackCooldown?.Invoke(fillAmount);

            if (attackTimer < 0.1f) {
                attackTimer = 0;
                AttackPerformed = false;
                OnAttackCooldown?.Invoke(1f);
            }
        }

    }

    private void Orbitate(Vector2 inputMovement) {

        Vector3 joystickDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 movementDirection = transform.TransformDirection((joystickDirection));

        Vector3 movement = new Vector3(0, 0, 0);

        float targetSpeed = movementDirection.magnitude * maxMovementSpeed;

       

        if (targetSpeed > 0) {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationTime);
            movement = movementDirection.normalized * currentSpeed * speedLimitMultiplier * Time.deltaTime;
            last_movement = movement;
            RotateToTarget(lookRotationSpeed, Time.deltaTime);
        } else {

            if (currentSpeed > 1) {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * deaccelerationTime);
            } else {
                currentSpeed = 0;
            }
            movement = last_movement.normalized * currentSpeed * speedLimitMultiplier * Time.deltaTime;
            RotateToTarget(lookRotationSpeed, Time.deltaTime);
        }

        if (!GameManager.instance.IsGameOnCombat()) {

            if (IsWalking == true) {
                IsWalking = false;
            }

            return; 
        }

        if (movementDirection.magnitude > 0) {
            isWalking = true;
        } else {
            isWalking = false;
        }

        characterController.Move(movement);
    }
   
    private void RotateToTarget(float followRotationSpeed, float timeDelta) {
        if (hasCombatStarted == true) {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, timeDelta * followRotationSpeed);
        }
    }

    private void HandleGestureInput(object sender, GestureInput.SwipeDirectionChangedEventArgs e) {

        if (player.Stamina > 0 && !AttackPerformed && !IsUsingItem && !DashPerformed) {

            switch (e.swipeDirection) {

                case SwipeDir.Left:
                    attacker.Attack("Swing_Left");
                    OnAttack?.Invoke("Swing_Left");
                    break;
                case SwipeDir.Up:
                    attacker.Attack("Swing_Stab");
                    OnAttack?.Invoke("Swing_Stab");
                    break;
                case SwipeDir.Down:
                    attacker.Attack("Swing_Down");
                    OnAttack?.Invoke("Swing_Down");
                    break;
                case SwipeDir.Right:
                    attacker.Attack("Swing_Right");
                    OnAttack?.Invoke("Swing_Right");
                    break;
                case SwipeDir.UpRight:
                    attacker.Attack("Swing_Right");
                    OnAttack?.Invoke("Swing_Right");
                    break;
                case SwipeDir.UpLeft:
                    attacker.Attack("Swing_Stab");
                    OnAttack?.Invoke("Swing_Stab");
                    break;
                case SwipeDir.DownRight:
                    attacker.Attack("Swing_Down");
                    OnAttack?.Invoke("Swing_Down");
                    break;
                case SwipeDir.DownLeft:
                    attacker.Attack("Swing_Left");
                    OnAttack?.Invoke("Swing_Left");
                    break;

                default:
                    break;
            }
            player.DrainStamina(player.AttackStaminaCost);
            AttackPerformed = true;
            attackTimer = attackCooldown;
        }
    }


    private void HandleGyroscopeInput() {
        // Gyroscope input logic goes here...
    }

    private void Dash(object sender, Joystick.OnDoubleTapEventArgs e) {

        //if (!GameManager.instance.IsGameOnCombat()) {
        //    if (IsWalking == true) {
        //        IsWalking = false;
        //    }
        //    return;
        //}

        if (!DashPerformed && player.Stamina > 0 && !AttackPerformed) {
            DashPerformed = true;
            OnDash?.Invoke((int)e.point.x);
            player.DrainStamina(player.MovementStaminaCost);
            StartCoroutine(DashRoutine(e.point));
            GetComponentInChildren<CharacterSoundFXManager>().PlayQuickStepSound();
        }
    }

    private IEnumerator DashRoutine(Vector2 point) {

        float duration = dashDistance / dashMovementSpeed; // Duration in seconds

        Vector3 startPosition = characterController.transform.position;
        Vector3 targetPosition = startPosition + (characterController.transform.forward * point.y + characterController.transform.right * point.x).normalized * dashDistance;

        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

            characterController.Move(currentPosition - characterController.transform.position);

            RotateToTarget(lookRotationSpeed * lookRotationSpeedMultiplier, t);

            elapsedTime += Time.fixedDeltaTime;

            yield return null;
        }

        DashPerformed = false;
    }

    private void ToggleIsUsingItem(bool value) {
        IsUsingItem = value;
        if (IsUsingItem) {
            speedLimitMultiplier = .5f;
        } else {
            speedLimitMultiplier = 1f;
        }
    }


    private IEnumerator LoadScenInputEvents() {

        GestureInput _gestureInput = null;
        Joystick _joystick = null;

        while (_gestureInput == null || _joystick == null) {
            _gestureInput = GameObject.Find("Gesture Input").GetComponent<GestureInput>();
            _joystick  = GameObject.Find("Joystick").GetComponent<Joystick>();
            yield return null;
        }

        gestureInput = _gestureInput;
        joystick = _joystick;

        gestureInput.SwipeDirectionChanged += HandleGestureInput;
        joystick.OnDoubleTap += Dash;    
    }

    public void LoadInputReferences() {
        StartCoroutine(LoadScenInputEvents());
    }

    public void LoadPlayerAttackSettings() {
        attackCooldown = player.AttackCooldown;

        attacker.UpdateComboTimerLimit(attackCooldown);

        attacker.UpdateDamage(player.Damage);
    }

    public void SetLockTarget(Transform tran) {
        target = tran;
    }
}

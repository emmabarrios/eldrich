using System;
using System.Collections;
using UnityEngine;

public class Player : Character, IDamageable
{
    public static Player instance;

    public enum PlayerState {Enabled, Disabled}
    public PlayerState currentState { get; set; }

    public void SetState(PlayerState newState) {
        if (newState == currentState) return;
        currentState = newState;
    }

    [SerializeField] private bool canRecoverStamina = false;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float beginHealthRecoverDelay;

    [Header("Stamina")]
    [SerializeField] private float staminaRecoverySpeed = 1f;
    [SerializeField] private float staminaRecoveryDelay = 1f;
    [SerializeField] private float staminaRecoverySpeedModifier = 1f;
    [SerializeField] private float attackStaminaCost;
    [SerializeField] private float movementStaminaCost;

    // Attack cooldown from weapon
    [Header("Base cooldown, testing")]
    [SerializeField] private float attackCooldown;

    public float AttackStaminaCost { get { return attackStaminaCost; } set { attackStaminaCost = value; } }
    public float MovementStaminaCost { get { return movementStaminaCost; } set { movementStaminaCost = value; } }
    public float StaminaRecoverySpeed { get { return staminaRecoverySpeed; } set { staminaRecoverySpeed = value; } }
    public float StaminaRecoveryModifier{ get { return staminaRecoverySpeedModifier; } set { staminaRecoverySpeedModifier = value; } }
    public float BeginHealthRecoverDelay { get { return beginHealthRecoverDelay; } set { beginHealthRecoverDelay = value; } }
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    public float MaxStamina { get { return maxStamina; } set { maxStamina = value; } }
    public float AttackCooldown { get { return attackCooldown; } set { attackCooldown = value; } }

    [SerializeField] private bool isBlocking = false;

    public Action OnDamageTaken;
    public Action OnHitBlocked;
    public Action<float, float> OnHealthValueRestored;
    public Action<float> OnHealthValueReduced;

    [SerializeField] private CharacterSoundFXManager characterSoundFXManager;
    [SerializeField] PlayerStatsManager characterStats = null;

    public event EventHandler<OnWeaponHitDetectedEventArgs> OnWeaponHitDetected;
    public event EventHandler<OnStaminaValueChanged_EventArgs> OnStaminaValueChanged;
    public class OnHealthValueChanged_EventArgs {
        public float value;
    }
    public class OnStaminaValueChanged_EventArgs {
        public float value;
    }
    public class OnWeaponHitDetectedEventArgs : EventArgs {
        public float damage;
    }


    private void Awake() {
        if (instance != null) { return; }
        instance = this;
    }

    private void Start() {
        currentState = PlayerState.Disabled;
        MaxHealth = Health;
        MaxStamina = Stamina;
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
       
        //StartCoroutine(EntranceCoroutine(GameManager.instance.entranceTime));
    }

    private void Update() {
        RecoverStaminaContinuously();
    }

    private void RecoverStaminaContinuously() {
        if (Stamina < MaxStamina && canRecoverStamina) {

            Stamina += StaminaRecoverySpeed * Time.deltaTime;

            OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });

            if (Stamina > MaxStamina) {
                Stamina = MaxStamina;
            } else if (Stamina < -20) {
                Stamina = -20;
            }
        }
    }

    public void DrainStamina(float staminaCost) {
        this.Stamina -= staminaCost;
        OnStaminaValueChanged?.Invoke(this, new OnStaminaValueChanged_EventArgs { value = Stamina });
        canRecoverStamina = false;
        StartCoroutine(DelayStaminaRecover(staminaRecoveryDelay));
    }

    public void RecoverHealth(float value) {
        StartCoroutine(RecoverHealthCoroutine(value, BeginHealthRecoverDelay));
    }

    public void TakeDamage(float damage) {
        Health -= damage;
        if (Health < 0f) {
            Health = 0f;
        }
        OnDamageTaken?.Invoke();
        OnHealthValueReduced?.Invoke(Health);
        //characterSoundFXManager.PlayDamageSoundFX();
    }

    private IEnumerator DelayStaminaRecover(float time) {
        yield return new WaitForSeconds(time);
        canRecoverStamina = true;
    }

    private IEnumerator RecoverHealthCoroutine(float healthAmmount, float delay) {

        yield return new WaitForSeconds(delay);

        float _tempHealth = 0f;

        if (Health + healthAmmount > MaxHealth) {
            _tempHealth = Mathf.Min(Health + healthAmmount, MaxHealth) - Health;
        } else {
            _tempHealth = healthAmmount;
        }

        Health += _tempHealth;

        if (Health > MaxHealth) {
            Health = MaxHealth;
        }

        float targetValue = Health;
        float currentValue = Health - _tempHealth;

        OnHealthValueRestored?.Invoke(currentValue, targetValue);
    }

    private IEnumerator EntranceCoroutine(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        SetState(PlayerState.Enabled);
    }

}

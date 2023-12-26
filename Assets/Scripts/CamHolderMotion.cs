using System.Collections;
using UnityEngine;

public class CamHolderMotion : MonoBehaviour
{
    private Animator animator;
    private Quaternion originalRotation;
    private Transform cameraTransform;
    [SerializeField] private Player player = null;
    //[SerializeField] private PlayerAnimator playerAnimator = null;
    Controller controller = null;

    [SerializeField] private GameObject enemy;

    [Header("Shake Settings")]
    [SerializeField] private float duration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float magnitude;
    [SerializeField] private int direction = 1;
    [SerializeField] private bool isrotating = false;

    [Header("Bobb Settings")]
    [SerializeField] private float bobDuration = 1f;
    [SerializeField] private float bobHeight = 1f;
    [SerializeField] private float pointXThreshold = 1f;
    private bool isBobbing = false;

    private Vector3 originalPosition;

    void Start()
    {
        //player = GameObject.Find("Player").GetComponent<Player>();
        //controller = player.GetComponent<Controller>();
        animator = GetComponent<Animator>();
        
        cameraTransform = GetComponent<Transform>();
        originalRotation = cameraTransform.localRotation;

        // Player Controller Event Subscribers
        //player.OnDamageTaken += StartCameraShake;
        //controller.OnDash += StartCameraBob;
        //controller.OnAttack += LookDirection;
        //playerAnimator.OnSwingLeft += PlayRotateLeftAnimation;
        //playerAnimator.OnSwingRight += PlayRotateRightAnimation;
        originalPosition = transform.localPosition;

        StartCoroutine(LoadScenePlayerEvents());
    }

    private void LookDirection(string dir) {
        switch (dir) {

            case "Swing_Left":
                animator.Play("look_left", -1, 0);
                break;
            case "Swing_Stab":
                animator.Play("look_up", -1, 0);
                break;
            case "Swing_Right":
                animator.Play("look_right", -1, 0);
                break;
            case "Swing_Down":
                animator.Play("look_down", -1, 0);
                break;
            default:
                break;
        }
    }

    public void StartCameraBob(int direction) {

        if (!GameManager.instance.IsGameOnCombat()) { return; }

        if (Mathf.Abs(direction) > pointXThreshold) {
            if (direction > 0) {
                animator.Play("rotate_right");
            } else {
                animator.Play("rotate_left");
            }
        }
        StartCoroutine(BobCamera(direction));
    }

    private void PlayRotateLeftAnimation() {
        animator.Play("rotate_left");
    }
    
    private void PlayRotateRightAnimation() {
        animator.Play("rotate_right");
    }

    public void StartCameraShake() {
        if (isrotating == false) {
            StartCoroutine(ShakeCamera( magnitude, direction));
        }
    }

    private IEnumerator ShakeCamera(float magnitude, int direction) {

        // Take away controll from the animator controller
        GetComponent<Animator>().enabled = false;

        Quaternion rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

        //Reset the camera's rotation to the original rotation
        cameraTransform.localRotation = originalRotation;

        rotation = originalRotation * Quaternion.Euler(0f, 0f, magnitude * direction);

        cameraTransform.localRotation = rotation;

        while (cameraTransform.localRotation != originalRotation) {
            // Apply the rotation towards the original rotation
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, originalRotation, Time.fixedDeltaTime * rotationSpeed);

            if (Mathf.Abs(cameraTransform.localRotation.z - originalRotation.z) < 0.01) {
                cameraTransform.localRotation = originalRotation;
            }
            yield return null;
        }
        // Return controll from the animator controller
        GetComponent<Animator>().enabled = true;
    }
    
    private IEnumerator BobCamera(int dir) {
        isBobbing = true;
        float elapsedTime = 0f;

        while (elapsedTime < bobDuration) {
            float progress = elapsedTime / bobDuration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * bobHeight;

            transform.localPosition = originalPosition - new Vector3(0f, yOffset, 0f);

            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isBobbing = false;
    }


    private IEnumerator LoadScenePlayerEvents() {

        Player _player = null;

        while (_player == null) {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            yield return null;
        }

        yield return new WaitUntil(PlayerComponentIsNotNull);

        bool PlayerComponentIsNotNull() {
            return _player.GetComponent<Player>() != null;
        }

        player = _player;
        controller = player.GetComponent<Controller>();

        player.OnDamageTaken += StartCameraShake;
        controller.OnDash += StartCameraBob;
        controller.OnAttack += LookDirection;
    }

    public void LoadInputReferences() {
        StartCoroutine(LoadScenePlayerEvents());
    }

}

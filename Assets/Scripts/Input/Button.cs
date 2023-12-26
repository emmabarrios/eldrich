using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private bool isActive = false;

    public bool IsActive { get { return isActive; } set { isActive = value; }
    }

    public bool toggleValue = false;

    public event EventHandler<OnBlockingEventArgs> OnBlocking;
    public event EventHandler OnHandleDroped;
    public Action<bool> OnToggleValueChanged;

    private Controller playerController;

    public class OnBlockingEventArgs: EventArgs {
        public string a;
    }

    private void Start() {
        playerController = GameObject.Find("Player").GetComponent<Controller>();
        playerController.OnParry += ChangeToggleValue;
    }

    public void OnPointerDown(PointerEventData eventData) {
        OnBlocking?.Invoke(this, new OnBlockingEventArgs { a = "Blocking" });
    }

    public void OnPointerUp(PointerEventData eventData) {
        toggleValue = !toggleValue;
        OnToggleValueChanged?.Invoke(toggleValue);
        OnHandleDroped?.Invoke(this, EventArgs.Empty);
    }

    private void ChangeToggleValue() {
        toggleValue = !toggleValue;
    }
}
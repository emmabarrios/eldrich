using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameFreeze : MonoBehaviour
{
    [Range(0f, 1.5f)]
    [SerializeField] private float freezeDuration = 1f;
    [SerializeField] private float pendingFreezeDuration = 0f;
    [SerializeField] bool isFrozen = false;

    private void Update() {
        if (pendingFreezeDuration < 0 && !isFrozen) {
            StartCoroutine(DoFreeze());
        }
    }

    private IEnumerator DoFreeze() {
        isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeDuration);
        Time.timeScale = original;
        pendingFreezeDuration = 0f;
        isFrozen = false;
    }

    public void Freeze() {
        pendingFreezeDuration = freezeDuration;
    }
}

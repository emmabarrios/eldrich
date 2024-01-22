using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] private float translationSpeed; // Speed of translation on the Y-axis (in units per second)
    [SerializeField] private float lifeSpam; // Speed of translation on the Y-axis (in units per second)

    public void DestroyAfterSeconds() {
        Destroy(gameObject, lifeSpam);
    }

    private void Update() {
        TranslateOverTime();
    }

    void Start() {
        DestroyAfterSeconds(); // Default destroy time, can be overridden by ImageSpawner
    }

    private void TranslateOverTime() {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = initialPosition + Vector2.up * translationSpeed * Time.deltaTime;

        rectTransform.anchoredPosition = targetPosition;
    }
}

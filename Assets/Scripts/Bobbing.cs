using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public Controller player;
    public float bobbingSpeed = 1f;
    public float bobbingAmount = 0.1f;

    private float originalY;
    private float timer = 0f;

    private void Start() {
        originalY = transform.localPosition.y;
    }

    private void Update() {
        float speed = player.CurrentSpeed;

        if (player.IsWalking) {
            float bobbingOffset = Mathf.Sin(timer) * bobbingAmount * speed;
            transform.localPosition = new Vector3(transform.localPosition.x, originalY + bobbingOffset, transform.localPosition.z);

            timer += bobbingSpeed * Time.deltaTime;
        }
       
    }
}

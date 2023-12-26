using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageFill : MonoBehaviour
{
    public Image imageToFill;
    public bool coolingDown;
    public float waitTime;

    public Controller controller = null;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScenePlayerEvents());

        //controller.OnAttackCooldown += UpdateFill;
        //controller.OnAttack += ResetFill;
        //ResetFill(1f);
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (coolingDown == true) {
        //    imageToFill.fillAmount += 1.0f / waitTime * Time.deltaTime;
        //    if (imageToFill.fillAmount >= 1f) {
        //        coolingDown = false;
        //    }
        //}
    }

    private void UpdateFill(float cooldown) {
        imageToFill.fillAmount = cooldown;
    }

    public void ResetFill(float coolDown) {
        imageToFill.fillAmount = 0f;
        waitTime = coolDown;
        coolingDown = true;
    }

    private void OnEnable() {
        imageToFill.fillAmount = 1f;
    }

    private IEnumerator LoadScenePlayerEvents() {

        Controller _controller = null;

        while (_controller == null) {
            _controller = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
            yield return null;
        }
        controller = _controller;

        controller.OnAttackCooldown += UpdateFill;
    }
}

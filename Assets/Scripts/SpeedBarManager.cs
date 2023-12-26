using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpeedBarManager : MonoBehaviour
{

    public Image image;
    public float speed = 100f;
    public Joystick joystick;

    float lerpSpeed;
    public float lerpSpeedValue;

    float globalMagnitude;

    void Start()
    {
        Reset();
        joystick = GameObject.Find("Dynamic Joystick").GetComponent<Joystick>();
        joystick.OnHandleDraged += UpdateFill;
        joystick.OnHandleDroped += Reset;
    }

    // Update is called once per frame
    void Update()
    {
        lerpSpeed = lerpSpeedValue * Time.deltaTime;

        image.fillAmount = Mathf.Lerp(image.fillAmount, globalMagnitude, lerpSpeed);
        //Color spedBarColor = Color.Lerp(Color.green, Color.red, (globalMagnitude / 100f));
        Color spedBarColor = Color.Lerp(Color.green, Color.red, (globalMagnitude / 100f));
        image.color = spedBarColor;

        if (image.fillAmount > 1) {
            image.fillAmount = 1;
        } else if (image.fillAmount < 0) {
            image.fillAmount = 0;
        }

    }

    public void UpdateFill(object sender, Joystick.OnHandleDragedEventArgs e) {
       

        //float mag = e.magnitude;
        //image.fillAmount = mag / 100f;
        //Color spedBarColor = Color.Lerp(Color.green, Color.red, (mag / 100f));
        //image.color = spedBarColor;

        /**/
        float mag = e.magnitude / 100f;
        globalMagnitude = mag;

        //image.fillAmount = Mathf.Lerp(image.fillAmount, mag, lerpSpeed);
        //Color spedBarColor = Color.Lerp(Color.green, Color.red, (mag / 100f));
        //image.color = spedBarColor;


        //if (image.fillAmount > 1) {
        //    image.fillAmount = 1;
        //} else if (image.fillAmount < 0) {
        //    image.fillAmount = 0;
        //}
    }

    public void Reset(object sender, EventArgs e) {
        //image.fillAmount = 0;
        globalMagnitude = 0;
    }

    public void Reset() {
        image.fillAmount = 0;
        globalMagnitude = 0;
    }

}

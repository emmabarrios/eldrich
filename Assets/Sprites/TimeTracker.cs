using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTracker : MonoBehaviour {

    public static TimeTracker instance;

    private System.DateTime currentTime;

    private int lastCheckedDay;

    public Action OnDayEnded;

    public System.DateTime CurrentTime { get { return currentTime; } }

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        currentTime = System.DateTime.Now;

        System.DateTime midnightTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);

        if (currentTime > midnightTime) {
            OnDayEnded?.Invoke();
        }
    }
}

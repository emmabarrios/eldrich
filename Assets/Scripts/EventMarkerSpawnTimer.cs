using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMarkerSpawnTimer : MonoBehaviour
{
    public static EventMarkerSpawnTimer instance;

    [SerializeField] private float totalTime = 60.0f; 
    [SerializeField] private float respawnTime;  
    [SerializeField] private bool isRunning = false;  


    void Start()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        respawnTime = 0;

        isRunning = false;
    }

    void Update()
    {
        if (isRunning) {
            respawnTime -= Time.deltaTime;
            if (respawnTime <= 0.0f) {
                StopTimer();
                Debug.Log("Timer reached 0");
            }
        }
    }

    public void StartTimer() {
        isRunning = true;
    }

    public void StopTimer() {
        isRunning = false;
    }

    public void ResetTimer() {
        respawnTime = totalTime;
        isRunning = true;
    }

    public bool IsTimerOut() {
        return respawnTime <= 0;
    }
}

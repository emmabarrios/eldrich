using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMarkerSpawnTimer : MonoBehaviour
{
    public static EventMarkerSpawnTimer instance;

    [SerializeField] private float totalTime = 60.0f;  // Total time for the timer in seconds
    [SerializeField] private float respawnTime;  // Remaining time on the timer
    [SerializeField] private bool isRunning = false;  // Flag to track whether the timer is currently running


    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        // Update the timer if it is currently running
        if (isRunning) {
            // Count down the remaining time
            respawnTime -= Time.deltaTime;

            // Check if the timer has expired
            if (respawnTime <= 0.0f) {
                // Stop the timer
                StopTimer();
                Debug.Log("Timer reached 0");
                // Invoke the callback function
                //onTimerExpired.Invoke();
            }
        }
    }

    // Start the timer
    public void StartTimer() {
        isRunning = true;
    }

    // Stop the timer
    public void StopTimer() {
        isRunning = false;
    }

    // Reset the timer to its initial state
    public void ResetTimer() {
        respawnTime = totalTime;
        isRunning = true;
    }

    public bool IsTimerOut() {
        return respawnTime <= 0;
    }
}

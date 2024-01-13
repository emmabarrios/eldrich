using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Unity.Location;
using TMPro;

public class TraveledDistanceTracker : MonoBehaviour
{
    public static TraveledDistanceTracker instance;
    [SerializeField]
    private DeviceLocationProviderAndroidNative locationProvider;

    [SerializeField]
    private double currentTraveledDistance;
    public double CurrentTraveledDistance { get { return currentTraveledDistance; } }

    public TextMeshProUGUI distanceUI;

    private int distanceExpPoints;

    public int meterExpLimit;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // Subscribe to the scene change event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Overworld") {
            locationProvider = GameObject.Find("AndroidDeviceLocationProvider").GetComponent<DeviceLocationProviderAndroidNative>();
            locationProvider.OnDistanceChange += UpdateCurrentTrveledDistance;

            distanceUI = GameObject.Find("Traveled Distance UI").GetComponent<TextMeshProUGUI>();
        }
    }

    private void UpdateCurrentTrveledDistance(double distance) {
        currentTraveledDistance += distance;

        if (Mathf.FloorToInt((float)currentTraveledDistance) % meterExpLimit == 0) {
            distanceExpPoints++;
            PlayerStatsManager.instance.UpdateExperience(distanceExpPoints);
            distanceExpPoints = 0;
        }

        distanceUI.text = $"Distance: {currentTraveledDistance:F2} meters";
    }
}

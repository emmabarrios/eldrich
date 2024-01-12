using TMPro;
using UnityEngine;
using Mapbox.Unity.Location;

public class DistanceTrackerUI : MonoBehaviour {
    public TextMeshProUGUI distanceText;
    public DeviceLocationProviderAndroidNative locationProvider;
    //private double currentTraveledDistance;
    //public double CurrentTraveledDistance { get { return currentTraveledDistance; } }

    void Start() {
        // If the locationProvider reference is not set, try to find it in the scene
        if (locationProvider == null) {
            locationProvider = FindObjectOfType<DeviceLocationProviderAndroidNative>();
        }

        if (locationProvider == null) {
            Debug.LogError("DeviceLocationProviderAndroidNative not found. Make sure it is in the scene or assign it manually.");
        }
    }

    void Update() {
        if (locationProvider != null) {
            // Update the TMPro Text with the totalTraveledDistance value
            distanceText.text = $"Total Traveled Distance: {locationProvider.totalTraveledDistance:F2} meters";
            //currentTraveledDistance = locationProvider.totalTraveledDistance;
        }
    }
}
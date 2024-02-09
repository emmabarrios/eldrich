using TMPro;
using UnityEngine;
using Mapbox.Unity.Location;

public class DistanceTrackerUI : MonoBehaviour {
    public TextMeshProUGUI distanceText;
    public DeviceLocationProviderAndroidNative locationProvider;

    void Start() {
        if (locationProvider == null) {
            locationProvider = FindObjectOfType<DeviceLocationProviderAndroidNative>();
        }

        if (locationProvider == null) {
            Debug.LogError("DeviceLocationProviderAndroidNative not found. Make sure it is in the scene or assign it manually.");
        }
    }

    void Update() {
        if (locationProvider != null) {
            distanceText.text = $"Total Traveled Distance: {locationProvider.totalTraveledDistance:F2} meters";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomPrefabSpawner : MonoBehaviour
{
    public static RandomPrefabSpawner instance;

    public List<GameObject> prefabList;  // List of prefabs to spawn
    public List<GameObject> lastInstantiatedPrefabList = new List<GameObject>();  // List of prefabs to spawn
    public float spawnRadius = 10f;  // Radius around the player to spawn prefabs
    public int numberOfPrefabs = 5;  // Number of prefabs to spawn
    public float minDistanceBetweenObjects = 2f;

    private EventMarkerSpawnTimer spawnTimer;

    [SerializeField] private Transform playerPin;

    [SerializeField] private Vector3 lastSpawnCenterPosition;

    void OnDrawGizmosSelected() {
        // Draw a wire sphere to represent the spawn radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    void Start() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        spawnTimer = GameObject.Find("EventMarkerSpawnTimer").GetComponent<EventMarkerSpawnTimer>();
        playerPin = GameObject.Find("PlayerPin").GetComponent<Transform>();

        lastSpawnCenterPosition = this.transform.position;

    }

    private void Update() {

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Overworld") {

            if (spawnTimer.IsTimerOut() && playerPin != null) {
                RemoveRemainingInstances();
                SpawnRandomPrefabs();
                ParentLastPrefabInstances();
                spawnTimer.ResetTimer();
            }
        }

        if (sceneName == "Overworld" && playerPin == null) {
            playerPin = GameObject.Find("PlayerPin").GetComponent<Transform>();
        }

        if (playerPin != null) {
            float distance = Mathf.Abs(Vector3.Distance(lastSpawnCenterPosition, playerPin.transform.position));
            if (distance > spawnRadius) {
                this.transform.position = playerPin.transform.position;
                lastSpawnCenterPosition = this.transform.position;
                RemoveRemainingInstances();
                SpawnRandomPrefabs();
                ParentLastPrefabInstances();
                spawnTimer.ResetTimer();
            }
        }
    }

    public void SpawnRandomPrefabs() {

        Vector3 playerPosition = playerPin.position;

        for (int i = 0; i < numberOfPrefabs; i++) {
            bool positionIsValid = false;

            // Calculate a random position within the specified radius
            Vector3 spawnPosition = Vector3.zero;

            while (!positionIsValid) {
                // Calculate a random position within the specified radius
                Vector3 randomOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                spawnPosition = playerPosition + randomOffset;

                // Check if the distance to other spawned objects is greater than the minimum distance
                positionIsValid = CheckDistanceToOtherObjects(spawnPosition, minDistanceBetweenObjects);

            }
            // Randomly select a prefab from the list
            GameObject selectedPrefab = prefabList[Random.Range(0, prefabList.Count)];

            // Randomly rotate the prefab along the y-axis
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);

            // Spawn the selected prefab at the calculated position with the random rotation
            Instantiate(selectedPrefab, spawnPosition, randomRotation);
        }

    }

    bool CheckDistanceToOtherObjects(Vector3 position, float minDistance) {
        // Check the distance to existing spawned objects
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Marker")) {
            if (Vector3.Distance(position, obj.transform.position) < minDistance) {
                return false; // Too close to another object
            }
        }
        return true; // Valid position
    }

    public void ParentLastPrefabInstances() {
        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Marker")) {
            obj.transform.SetParent(this.transform);
        }
    }

    public void RemoveRemainingInstances() {

        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void ToggleInstances(Vector3 exclusion) {

        for (int i = 0; i < transform.childCount; i++) {
            GameObject current = transform.GetChild(i).gameObject;
            if (current.GetComponent<EventMarker>().markerLocationId == exclusion) {
                Destroy(current);
            }
            current.SetActive(!current.activeSelf);
        }
    }
    
    public void ToggleInstances() {

        for (int i = 0; i < transform.childCount; i++) {
            GameObject current = transform.GetChild(i).gameObject;
            current.SetActive(!current.activeSelf);
        }
    }
}

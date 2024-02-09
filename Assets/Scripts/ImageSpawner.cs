using System.Collections;
using TMPro;
using UnityEngine;

public class ImageSpawner : MonoBehaviour
{
    public GameObject damageUI; // Drag your image prefab here in the Unity editor
    public Attacker attacker;
    public string _text;

    private void Awake() {
        StartCoroutine(WaitForPlayerAndSetupEvents());
    }

    private void SpawnImage(string text) {
        GameObject spawnedImage = Instantiate(damageUI, transform.position, Quaternion.identity);
        spawnedImage.transform.SetParent(this.transform);
        TextMeshProUGUI textMeshPro = spawnedImage.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = text;
    }

    private IEnumerator WaitForPlayerAndSetupEvents() {
        GameObject playerObject = null;

        while (playerObject == null) {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        Player player = playerObject.GetComponent<Player>();
        attacker = player.transform.GetComponentInChildren<Attacker>();
        attacker.OnAttackLanded += SpawnImage;
    }

}

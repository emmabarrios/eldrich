using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] GameObject character;

    public void SpawnCharacter() {
        GameObject spawnedCharacter = Instantiate(character, this.transform.position, this.transform.rotation);
    }
}

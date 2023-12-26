using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] AudioClip explosionSoundFX;
    [SerializeField]
    [Range(0f, 1f)]
    float volumeScale;

    AudioSource audioSource;
    

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        audioSource.PlayOneShot(explosionSoundFX, volumeScale);
    }
}

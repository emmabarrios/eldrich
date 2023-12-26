using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Swing Sounds FX")]
    [SerializeField][Range(0f, 1f)] float volumeScale;
    
    public AudioClip[] swordSwingsSounds;

    private List<AudioClip> potentialfootstepsSounds;
    private List<AudioClip> potentialSwordSwingsSounds;
    private AudioClip lastSoundPlayed;

    [Header("Hit Sounds FX")]
    public AudioClip takeDamageSound;
    public AudioClip blockHitSound;

    [Header("Movement Sounds FX")]
    public AudioClip[] footstepsSounds;
    public AudioClip sideStepSound;


    private List<AudioClip> potentialDamageSounds;
    private AudioClip lastDamageSoundPlayed;


    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDamageSoundFX() {
        audioSource.PlayOneShot(takeDamageSound, .4f);

        //potentialDamageSounds = new List<AudioClip>();
        //foreach (var damageSound in takeDamageSounds) {
        //    if (damageSound!=lastDamageSoundPlayed) {
        //        potentialDamageSounds.Add(damageSound);
        //    }
        //}
        //int randomValue = Random.Range(0, potentialDamageSounds.Count);
        //lastDamageSoundPlayed = takeDamageSounds[randomValue];
        //audioSource.PlayOneShot(takeDamageSounds[randomValue], .4f);
    }   
    
    public void PlayBlockSoundFX() {
        audioSource.PlayOneShot(blockHitSound, .4f);
    }

    public void PlayRandomFootstepSoundFX() {

        potentialfootstepsSounds = new List<AudioClip>();
        foreach (var damageSound in footstepsSounds) {
            if (damageSound != lastSoundPlayed) {
                potentialfootstepsSounds.Add(damageSound);
            }
        }
        int randomValue = UnityEngine.Random.Range(0, potentialfootstepsSounds.Count);
        lastSoundPlayed = footstepsSounds[randomValue];
        audioSource.PlayOneShot(footstepsSounds[randomValue], volumeScale);
    }

    public void PlayRandomSwingSoundFX() {

        potentialSwordSwingsSounds = new List<AudioClip>();
        foreach (var damageSound in swordSwingsSounds) {
            if (damageSound != lastSoundPlayed) {
                potentialSwordSwingsSounds.Add(damageSound);
            }
        }
        int randomValue = UnityEngine.Random.Range(0, potentialSwordSwingsSounds.Count);
        lastSoundPlayed = swordSwingsSounds[randomValue];
        audioSource.PlayOneShot(swordSwingsSounds[randomValue], volumeScale);
    }

    public void PlayQuickStepSound(){
        audioSource.PlayOneShot(sideStepSound, volumeScale);
    }
}

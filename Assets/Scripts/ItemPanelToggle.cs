using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanelToggle : MonoBehaviour
{
    public static ItemPanelToggle instance;

    [SerializeField] private GameObject buttonA = null;
    [SerializeField] private GameObject buttonB = null;
    [SerializeField] private GameObject QuickItemPanel = null;
    private Toggle toggle;

    public Action<bool> OnToggleValueChanged;

    [Header("Sound FX Settings")]
    [SerializeField][Range(0f, 1f)] float volumeScale;
    public AudioClip[] openSounds;
    private List<AudioClip> potentialOpenSounds;
    private AudioClip lastSoundPlayed;
    AudioSource audioSource;

    void Awake() {
        if (instance != null) { return; }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        toggle = this.GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle);
        });

        audioSource = this.GetComponent<AudioSource>();

    }

    void ToggleValueChanged(Toggle change) {
        if (buttonA!=null) {
            buttonA.SetActive(!change.isOn);
        }
        if (buttonB!=null) {
            buttonB.SetActive(!change.isOn);
        }
        OnToggleValueChanged?.Invoke(change.isOn);
        PlayRandomSoundFX();
    }


    public virtual void PlayRandomSoundFX() {

        potentialOpenSounds = new List<AudioClip>();
        foreach (var damageSound in openSounds) {
            if (damageSound != lastSoundPlayed) {
                potentialOpenSounds.Add(damageSound);
            }
        }
        int randomValue = UnityEngine.Random.Range(0, potentialOpenSounds.Count);
        lastSoundPlayed = openSounds[randomValue];
        audioSource.PlayOneShot(openSounds[randomValue], volumeScale);
    }

}

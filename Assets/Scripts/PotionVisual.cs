using System.Collections;
using UnityEngine;

public class PotionVisual : MonoBehaviour
{
    [SerializeField] private GameObject potionSeal;
    [Range(0f, 10f)] public float disableSealTimer;
    private bool isTiming;

    [Range(0f, 10f)] public float useTimer;

    // Start is called before the first frame update
    void Start()
    {
        isTiming = true;   
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming) {
            disableSealTimer -= Time.deltaTime;
            if (disableSealTimer < 0.1f) {
                disableSealTimer = 0;
                isTiming = false;
                potionSeal.SetActive(false);
            }
        }
    }

}

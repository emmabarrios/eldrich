using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class CombatUI : MonoBehaviour
{
    public Image healthImage;
    public Image staminaImage;
    public CombatUIcarousel carousel = null;

    [SerializeField][Range(.1f, 1f)] float healthBarReoverSpeed = 0.25f;

    public Player player;
    public float healthPoints;
    public float staminaPoints;

    // Start is called before the first frame update
    void Start()
    {
        //player.OnHealthValueReduced += ReduceHealthBar;
        //player.OnHealthValueRestored += FillHealthBar;
        //player.OnStaminaValueChanged += Update_StaminaBar;

        //healthPoints = player.Health;
        //staminaPoints = player.Stamina;

        //healthImage.fillAmount = healthPoints / healthPoints;
        //staminaImage.fillAmount = staminaPoints / staminaPoints;

        //StartCoroutine(LoadScenePlayerEvents());


        player.OnHealthValueReduced += ReduceHealthBar;
        player.OnHealthValueRestored += FillHealthBar;
        player.OnStaminaValueChanged += Update_StaminaBar;

        healthPoints = player.Health;
        staminaPoints = player.Stamina;

        healthImage.fillAmount = healthPoints / healthPoints;
        staminaImage.fillAmount = staminaPoints / staminaPoints;
    }

    private void ReduceHealthBar(float value) {
        healthImage.fillAmount = value / healthPoints;
    }
    
    private void Update_StaminaBar(object sender, Player.OnStaminaValueChanged_EventArgs e) {
        staminaImage.fillAmount = e.value / staminaPoints;
    }

    private void FillHealthBar(float currentValue, float targetValue) {
        StartCoroutine(FillHealthBarCoroutine(currentValue, targetValue));
    }
    private IEnumerator FillHealthBarCoroutine(float currentValue, float targetValue) {

        float startTime = Time.time;
        float endTime = startTime + healthBarReoverSpeed;

        while (Time.time < endTime) {
            float elapsedTime = Time.time - startTime;
            float progress = elapsedTime / healthBarReoverSpeed;

            currentValue = Mathf.Lerp(currentValue, targetValue, progress);
            healthImage.fillAmount = currentValue / healthPoints;
            yield return null;

        }
    }

    //private void OnEnable() {
    //    carousel.InitializeUIcarousel(CombatInventory.instance.itemLists);
    //}

    //private IEnumerator LoadScenePlayerEvents() {

    //    Player _player = null;

    //    while (_player == null) {
    //        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //        yield return null;
    //    }
    //    player = _player;

    //    player.OnHealthValueReduced += ReduceHealthBar;
    //    player.OnHealthValueRestored += FillHealthBar;
    //    player.OnStaminaValueChanged += Update_StaminaBar;

    //    healthPoints = player.Health;
    //    staminaPoints = player.Stamina;

    //    healthImage.fillAmount = healthPoints / healthPoints;
    //    staminaImage.fillAmount = staminaPoints / staminaPoints;
    //}
}

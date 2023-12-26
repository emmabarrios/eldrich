using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Combat Scene Characters")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    [SerializeField] private WorldEvent clickedEvent;
    [SerializeField] private Vector3 lastClickedEventMarkerLocationId;

    // Event to destroy object based on battle results
    public Action<string> OnCombatEventFinished;


    private bool isOnCombatScene = false;
    private bool isOnOverworldScene = false;
    private bool combatSceneLoaded = false;

    public enum GameStates {
        WaitingToStart,
        LoadingCombatAssets,
        Combat,
        CombatIntro,
        CombatOutro,
        Wait,
        Overworld,
        LoadingWorldAssets
    }


    public GameStates state;
    private float waitingToStartTimer = 1f;

    [Header("Combat Timers")]
    [SerializeField] private float countdownToStartCombatTimer = 3f;
    [SerializeField] private float combatLimitTimer;
    [SerializeField] private float combatLimitTimerMax = 10f;
    [SerializeField] private float combatOutroTimer = 3f;

    // Framerate limits
    public enum Limits {
        noLimit = 0,
        limit12 = 12,
        limit25 = 25,
        limit30 = 30,
        limit35 = 35,
        limit40 = 50,
        limit45 = 45,
        limit50 = 50,
        limit60 = 60
    }

    [Header("Framerate settings")]
    public Limits limits;

    [Header("Background music tracks")]
    public AudioClip battleAudio;
    AudioSource audioSource;

    [Header("Battle Settings")]
    public float entranceTime = 2f;

    [Header("Last Combat results")]
    public bool wasEnemyDefeated = false;

    private void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        state = GameStates.Wait;
    }

    private void Update() {

        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName) {

            case "MainScene":
                if (isOnCombatScene == false) {
                    isOnCombatScene = true;
                    isOnOverworldScene = false;

                    RandomPrefabSpawner.instance.ToggleInstances();
                    state = GameStates.CombatIntro;

                    // Instantiate enemy on place
                    Instantiate(enemy, GameObject.Find("EnemySpawnPoint").transform.position, Quaternion.identity);
                    PlayBattleBackgroundAudio();

                }
                break;
            
            case "Overworld":
                if (isOnOverworldScene == false) {
                    isOnOverworldScene = true;
                    isOnCombatScene = false;
                    state = GameStates.LoadingWorldAssets;
                    //UpdateOverworldUI();
                }
                break;

            default:
                break;
        }

        //if (combatSceneLoaded == false) {
        //    if (SceneManager.GetActiveScene().name.ToString() == "MainScene") {
        //        combatSceneLoaded = true;
        //        state = GameStates.CombatIntro;
        //    }
        //}
       

        // State management
        switch (state) {
            //case GameStates.WaitingToStart:
            //    waitingToStartTimer -= Time.deltaTime;
            //    if (waitingToStartTimer < 0) {
            //        state = GameStates.CombatIntro;
            //    }
            //    break;

            case GameStates.CombatIntro:
                countdownToStartCombatTimer -= Time.deltaTime;

                if (countdownToStartCombatTimer <= 0) {
                    combatLimitTimer = combatLimitTimerMax;
                    PlayerStatsManager.instance.LoadPlayerStats(Player.instance);
                    CombatInventory.instance.LoadPlayerWeapon();
                    GameObject.Find("Carousel").GetComponent<CombatUIcarousel>().InitializeUIcarousel(CombatInventory.instance.itemLists);

                    // Initialize player controller
                    GameObject.Find("Player").GetComponent<Controller>().LoadPlayerAttackSettings();


                    state = GameStates.Combat;
                    //ToggleCombatUI();
                    //InitializePlayer();
                }
                break;

            case GameStates.Combat:

                combatLimitTimer -= Time.deltaTime;

                if (combatLimitTimer < 0) {
                    EndCombatSequence(false);
                }
                break;

            case GameStates.CombatOutro:

                //combatOutroTimer -= Time.deltaTime;
                //if (combatOutroTimer < 0) {
                //    ToggleBattleResultsUI();
                //    state = GameStates.Wait;
                //}

                break;

            //case GameStates.LoadingCombatAssets:

            //    // Load the enemy game object
            //    // Load the player game object
            //    // Load player stats
            //    // Initialize combat UI carousell
            //    // Change state to combat intro

            //    //StartCoroutine(LoadCombatScene());

            //    if (!combatSceneLoaded) {
            //        StartCoroutine(LoadCombatScene());
            //        combatSceneLoaded = true;
            //    }

            //    break;

            case GameStates.Wait:
                break;

            case GameStates.Overworld:
                //if (playerWonLastEvent) {
                //    Debug.Log(clickedEvent.name);
                //} else {
                //    Debug.Log("Player lost");
                //}
                break;

            case GameStates.LoadingWorldAssets:
                if (lastClickedEventMarkerLocationId != Vector3.zero) {
                    if (wasEnemyDefeated) {
                        RandomPrefabSpawner.instance.ToggleInstances(lastClickedEventMarkerLocationId);
                    } else {
                        RandomPrefabSpawner.instance.ToggleInstances();
                    }
                    StopBattleAudioLoop();
                    
                }

                state = GameStates.Overworld;
                break;
        }
        //Debug.Log(state);
    }

    public void EndCombatSequence(bool wasEnemyDefeated) {
        
        this.wasEnemyDefeated = wasEnemyDefeated;
        Debug.Log(this.wasEnemyDefeated);
        ToggleCombatUI();
        ToggleBattleResultsUI(wasEnemyDefeated);

        // if wasEnemyDefeated, send exp to StatsManager and items to GeneralInventory
        if (wasEnemyDefeated) {
            GeneralInventory.instance.StoreItems(GetQuickItemList(), GeWeaponItemList());
            GeneralInventory.instance.SortItems();

            //PlayerStatsManager.instance.UpdateExperience(GetCombatExperience());
            PlayerStatsManager.instance.EarnedExperience += GetCombatExperience();
        }

        state = GameStates.CombatOutro;
    }

    void Start()
    {
        Application.targetFrameRate = (int)limits;
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(battleAudio);

        //Enemy enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    //public void InitializeCombatInventoryUI() {
    //   GameObject.Find("Carousel").GetComponent<CombatUIcarousel>().InitializeUIcarousel(CombatInventory.instance.itemLists);
    //}

    public void InitializePlayer() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().LoadInputReferences();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().SetLockTarget(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>());
        PlayerStatsManager.instance.LoadPlayerStats(Player.instance);
        CombatInventory.instance.LoadPlayerWeapon();
        
    }

    public bool IsGameOnCombat() {
        return state == GameStates.Combat;
    }

    private void ToggleCombatUI() {
        Transform playerCombatUI = GameObject.Find("Canvas").transform.GetChild(0);
        Transform enemyCombatUI = GameObject.FindGameObjectWithTag("Enemy").transform.GetChild(0);

        playerCombatUI.gameObject.SetActive(!playerCombatUI.gameObject.activeSelf);
        enemyCombatUI.gameObject.SetActive(!enemyCombatUI.gameObject.activeSelf);
    } 
    private void ToggleBattleResultsUI(bool combatResult) {
        Transform battleResultsVictoryUI = GameObject.Find("Canvas").transform.GetChild(1);
        Transform battleResultsDefeatUI = GameObject.Find("Canvas").transform.GetChild(2);

        if (combatResult) {
            battleResultsVictoryUI.gameObject.SetActive(!battleResultsVictoryUI.gameObject.activeSelf);
        } else {
            battleResultsDefeatUI.gameObject.SetActive(!battleResultsDefeatUI.gameObject.activeSelf);
        }
        
    }

    public float GetCombatTimerNormalized() {
        return  (combatLimitTimer / combatLimitTimerMax);
    }

    public void SetToOutroState() {
        ToggleCombatUI();
        state = GameStates.CombatOutro;
    }

    private IEnumerator LoadCombatScene() {
       
        yield return LoadCharacter(enemy, GameObject.Find("Enemy Spawn Point").transform);
        Debug.Log("Enemy spawned");

        yield return LoadCharacter(player, GameObject.Find("Player Spawn Point").transform);
        Debug.Log("Player spawned");

        yield return new WaitUntil(EnemyComponentIsNotNull);
        yield return new WaitUntil(PlayerComponentIsNotNull);

        bool EnemyComponentIsNotNull() {
            return GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>() != null;
        }
        bool PlayerComponentIsNotNull() {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() != null;
        }

        ToggleCombatUI();

        InitializePlayer();

        state = GameStates.CombatIntro;

    }

    private IEnumerator LoadCharacter(GameObject obj, Transform tsfm) {
        GameObject loadedCharacter = Instantiate(obj, tsfm.position, tsfm.rotation);

        // Wait until the loaded character's Awake and Start methods are called
        //yield return new WaitUntil(() => loadedCharacter.GetComponent<Character>() != null);

        yield return new WaitUntil(ObjectComponentIsNotNull);

        bool ObjectComponentIsNotNull() {
            return loadedCharacter.GetComponent<Character>() != null;
        }
    }

    public void SetGameManagerState(GameStates state) {
        this.state = state;
    }

    public void LoadEventProperties(WorldEvent clickedEvent, GameObject eventMarker) {
        this.clickedEvent = clickedEvent;
        enemy = clickedEvent._enemy;
        lastClickedEventMarkerLocationId = eventMarker.GetComponent<EventMarker>().markerLocationId;
    }

    public List<QuickItem> GetQuickItemList() {
        return this.clickedEvent._itemList;
    }
    public List<WeaponItem> GeWeaponItemList() {
        return this.clickedEvent._weaponItemList;
    }

    public int GetCombatExperience() {
        return clickedEvent._exp;
    }
    
    public void PlayBattleBackgroundAudio() {
        // Check if the audio source is not playing to avoid overlapping
        if (!audioSource.isPlaying) {
            audioSource.clip = battleAudio;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    public void StopBattleAudioLoop() {
        audioSource.Stop();
    }

    //private void UpdateOverworldUI() {
    //    GameObject.Find("Quick Item Card Container").GetComponent<QuickItemsCardGroup>().UpdateCardGroupContent(GeneralInventory.instance);
    //}
}

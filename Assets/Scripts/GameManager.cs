using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Combat Scene Characters")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    private bool isEnemyPlaced;

    [SerializeField] private WorldEvent clickedEvent;
    [SerializeField] private Vector3 lastClickedEventMarkerLocationId;

    // Event to destroy object based on battle results
    public Action<string> OnCombatEventFinished;

    public GameStates state;
    private float waitingToStartTimer = 1f;

    [Header("Combat Timers")]
    [SerializeField] private float countdownToStartCombatTimer = 3f;
    [SerializeField] private float combatLimitTimer;
    [SerializeField] private float combatLimitTimerMax = 10f;
    [SerializeField] private float combatOutroTimer = 3f;

    [Header("Framerate settings")]
    public Limits limits;

    [Header("Background music tracks")]
    public AudioClip battleAudio;
    public AudioClip overworldAudio;
    AudioSource audioSource;

    [Header("Battle Settings")]
    public float entranceTime = 2f;

    [Header("Last Combat results")]
    public bool wasEnemyDefeated = false;

    [Header("Volumen Settings")]
    public float globalVolume = 1.0f; // Default volume is 100%
    public float GlobalVolume {
        get { return globalVolume; }
        set {
            globalVolume = Mathf.Clamp01(value); // Ensure volume is between 0 and 1
            UpdateAllAudioSources();
        }
    }

    [Header("Map loading threshold")]
    public int allowedSecconds = 10;

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
             
        switch (state) {

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
                }
                break;

            case GameStates.Combat:

                combatLimitTimer -= Time.deltaTime;

                if (combatLimitTimer < 0) {
                    EndCombatSequence(false);
                    //state = GameStates.Wait;
                }
                break;

            default:
                break;

        }

    }

    //private void OnApplicationPause(bool pauseStatus) {
    //    if (pauseStatus) {
    //        // The application is paused
    //        DatabaseManager.instance.SaveGame();
    //    }
    //}

    //private void OnApplicationQuit() {
    //    // The application is being closed
    //    DatabaseManager.instance.SaveGame();
    //}

    public void EndCombatSequence(bool wasEnemyDefeated) {
        
        this.wasEnemyDefeated = wasEnemyDefeated;
        ToggleCombatUI();
        ToggleBattleResultsUI(wasEnemyDefeated);

        // if wasEnemyDefeated, send exp to StatsManager and items to GeneralInventory
        if (wasEnemyDefeated) {
            GeneralInventory.instance.StoreItems(GetLoot());
            PlayerStatsManager.instance.EarnedExperience += GetCombatExperience();
        }
        state = GameStates.Wait;
        DatabaseManager.instance.SaveGame();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        Application.targetFrameRate = (int)limits;

        // Block screen with loading panel
        LoadingPanel.instance.ShowLoadingScreen();

        // TEST FOR INFINITE LOOP BUG
        LoadUserData();

        LoadingPanel.instance.ShowLoadingScreen();
        StartCoroutine(WaitForMapTilesCoroutine());

        StartCoroutine(WaitForMapTilesCoroutine());
        // If everything was loaded, here the panel will hide

        SwitchBackgroundTrack(overworldAudio);

    }


    public async void LoadUserData() {

        bool userDataLoadedSuccessfully = await DatabaseManager.instance.LoadUserData_();

        if (!userDataLoadedSuccessfully) {
            GameObject.Find("Canvas").GetComponent<OverworldUIManager>().ToggleErrorLoadingPanel();
        } 

    }

    private IEnumerator WaitForMapTilesCoroutine() {
        int mapTiles = 0;
        int seccondsPassed = 0;
        while (mapTiles <= 1) {
            seccondsPassed++;
            mapTiles = GameObject.Find("Map").transform.childCount;
            if (seccondsPassed>5) {
                LoadingPanel.instance.UpdateLoadingMessage("Map loading is taking longer than expected...");
            }
            yield return new WaitForSeconds(1);
        }
        LoadingPanel.instance.HideLoadingScreen();
    }

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

    //private IEnumerator LoadCharacter(GameObject obj, Transform tsfm) {
    //    GameObject loadedCharacter = Instantiate(obj, tsfm.position, tsfm.rotation);

    //    yield return new WaitUntil(ObjectComponentIsNotNull);

    //    bool ObjectComponentIsNotNull() {
    //        return loadedCharacter.GetComponent<Character>() != null;
    //    }
    //}

    //public void SetGameManagerState(GameStates state) {
    //    this.state = state;
    //}

    public void LoadEventProperties(WorldEvent clickedEvent, GameObject eventMarker) {
        this.clickedEvent = clickedEvent;
        enemy = clickedEvent._enemy;
        lastClickedEventMarkerLocationId = eventMarker.GetComponent<EventMarker>().markerLocationId;
    }

    public List<Item> GetLoot() {
        return this.clickedEvent._loot;
    }

    public int GetCombatExperience() {
        return clickedEvent._exp;
    }
    
    public void StopBackgroundAudioLoop() {
        audioSource.Stop();
    }

    private void SwitchBackgroundTrack(AudioClip clip) {
        if (audioSource.clip != clip || !audioSource.isPlaying) {
            audioSource.Stop(); // Stop the currently playing track
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

   

    private void UpdateAllAudioSources() {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources) {
            audioSource.volume = globalVolume;
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
        string sceneName = scene.name;

        switch (sceneName) {
            case("Overworld"):
                LoadingPanel.instance.ShowLoadingScreen();

                StartCoroutine(WaitForMapTilesCoroutine());

                SwitchBackgroundTrack(overworldAudio);

                UpdateAllAudioSources();

                if (lastClickedEventMarkerLocationId != Vector3.zero) {
                    if (wasEnemyDefeated) {
                        RandomPrefabSpawner.instance.ToggleInstances(lastClickedEventMarkerLocationId);
                    } else {
                        RandomPrefabSpawner.instance.ToggleInstances();
                    }
                }

                //state = GameStates.Overworld;

                //DatabaseManager.instance.SaveGame();

                break;
            case ("MainScene"):
                RandomPrefabSpawner.instance.ToggleInstances();

                UpdateAllAudioSources();

                SwitchBackgroundTrack(battleAudio);

                Instantiate(enemy, GameObject.Find("EnemySpawnPoint").transform.position, Quaternion.identity);

                state = GameStates.CombatIntro;

                break;
            default:

                break;
        }
    }
}

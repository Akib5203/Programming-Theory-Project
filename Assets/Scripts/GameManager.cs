using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_applicationIsQuiting) return null;

            if (_instance == null) _instance = FindObjectOfType<GameManager>();

            if (_instance == null)
            {
                GameObject gameManager = new("Game Manager", typeof(GameManager));
                DontDestroyOnLoad(gameManager);
                _instance = gameManager.GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    private static bool _applicationIsQuiting = false;


    // Achievement Trackers
    private float _distanceTravelled;
    private int _punchCount;
    private int _kickCount;
    private int _totalHitsCount;

    // Local events to handle UI and settings
    public System.Action<bool> Loading;
    public System.Action SavedNotification;
    public System.Action<DistanceAchievement> RunnerAchievementNotification;
    public System.Action<CombatAchievement> CombatAchievementNotification;
    public System.Action<AchievementType, CombatType, int, bool> AchievementUpdate;
    public System.Action StatsReadyForDisplay;

    // Achievement Tracker Events
    public System.Action<int> DistanceTravelledChanged;
    public System.Action<int> PunchCountChanged;
    public System.Action<int> KickCountChanged;
    public System.Action<int> TotalHitsCountChanged;

    public DistanceAchievement CachedOldRunnerAchievement;
    public CombatAchievement CachedOldCombatAchievement;

    private readonly string _settingsPrefName = "BasicGameSettings";

    // Counts for UI and Tracking purposes
    public float DistanceTravelled 
    {
        get => _distanceTravelled;
        set
        {
            if (_distanceTravelled != value)
            {
                _distanceTravelled = value;
                DistanceTravelledChanged.Invoke((int)_distanceTravelled); // Whenever the value changes we invoke the action.
            }
        }
    }
    public int PunchCount
    {
        get => _punchCount;
        set
        {
            if (_punchCount != value)
            {
                _punchCount = value;
                PunchCountChanged.Invoke(_punchCount); // Whenever the value changes we invoke the action.
            }
        }
    }
    public int KickCount
    {
        get => _kickCount;
        set
        {
            if (_kickCount != value)
            {
                _kickCount = value;
                KickCountChanged.Invoke(_kickCount); // Whenever the value changes we invoke the action.
            }
        }
    }
    public int TotalHitsCount
    {
        get => _totalHitsCount;
        set
        {
            if (_totalHitsCount != value)
            {
                _totalHitsCount = value;
                TotalHitsCountChanged.Invoke(_totalHitsCount); // Whenever the value changes we invoke the action.
            }
        }
    }

    // This method runs whenever the Scene is loaded
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetQuitFlag() => _applicationIsQuiting = false; // Set application is quitting flag to false, because application is running

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void StartGameManager() => Debug.Log("Game Manager on duty!");

    private void Start()
    {
        // Invoke the Action that Stats are ready to display in the UI
        StatsReadyForDisplay.Invoke();
    }

    public void SaveBasicSettings(BasicGameSettings settings)
    {
        string json = JsonUtility.ToJson(settings);

        PlayerPrefs.SetString(_settingsPrefName, json);
        PlayerPrefs.Save();

        SavedNotification.Invoke();
    }

    public BasicGameSettings LoadBasicSettings()
    {
        if (!PlayerPrefs.HasKey(_settingsPrefName)) SaveBasicSettings(new BasicGameSettings() { AudioVolume = 0.3f, CameraDistance = 2f, CameraSide = 0.559f });
        string json = PlayerPrefs.GetString(_settingsPrefName);
        BasicGameSettings settings = JsonUtility.FromJson<BasicGameSettings>(json);
        return settings;
    }

    public DistanceAchievement LoadInitialDistanceAchievementStates()
    {
        DistanceAchievement distanceAchievement = new()
        {
            Meters10 = false, Meters100 = false, Meters1000 = false, Meters20 = false, Meters30 = false, Meters40 = false, Meters50 = false, Meters500 = false, Meters60 = false, Meters70 = false, Meters80 = false, Meters90 = false
        };
        return distanceAchievement;
    }

    public CombatAchievement LoadInitialCombatAchievementStates()
    {
        CombatAchievement combatAchievement = new()
        {
            Punches10 = false, Punches100 = false, Hits100 = false, Hits20 = false, Hits40 = false, Hits500 = false, Kicks10 = false, Kicks100 = false, Kicks20 = false, Kicks50 = false, Punches20 = false, Punches50 = false
        };
        return combatAchievement;
    }

    private void OnDestroy() => _applicationIsQuiting = true; // On Gameobject Destroyed set application is quitting flag to true
}

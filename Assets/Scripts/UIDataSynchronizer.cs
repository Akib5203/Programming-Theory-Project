using UnityEngine;

public class UIDataSynchronizer : MonoBehaviour
{
    [Header("Basic Setting Handler")]
    [SerializeField] private SettingsHandler _settingHandler;

    [Header("Achievement State Handlers")]
    [SerializeField] private AchievementStateHandler[] _achievementStateHandlers;

    [Header("Firestore Property Names")]
    [SerializeField] private System.Collections.Generic.List<AchievementKeyValuePair> _runnerProperties;
    [SerializeField] private System.Collections.Generic.List<AchievementKeyValuePair> _combatProperties;

    [Header("Starter Asset Input")]
    [SerializeField] private StarterAssets.StarterAssetsInputs _inputs;

    private DistanceAchievement _distanceAchievements;
    private CombatAchievement _combatAchievements;

    private System.Collections.Generic.Dictionary<(CombatType, int), string> _runnerPropertiesDict = new();
    private System.Collections.Generic.Dictionary<(CombatType, int), string> _combatPropertiesDict = new();

    private void Awake()
    {
        // Synchronize List to Dictionary
        _runnerPropertiesDict.Clear();
        _combatPropertiesDict.Clear();
        foreach (AchievementKeyValuePair kvp in _runnerProperties)
        {
            if (kvp.AchievementGoal != 0 && !_runnerPropertiesDict.ContainsKey((kvp.CombatType, kvp.AchievementGoal)))
                _runnerPropertiesDict[(kvp.CombatType, kvp.AchievementGoal)] = kvp.PropertyName;
        }
        
        foreach (AchievementKeyValuePair kvp in _combatProperties)
        {
            if (kvp.AchievementGoal != 0 && !_combatPropertiesDict.ContainsKey((kvp.CombatType, kvp.AchievementGoal)))
                _combatPropertiesDict[(kvp.CombatType, kvp.AchievementGoal)] = kvp.PropertyName;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.StatsReadyForDisplay += OnStatsReadyForDisplay; //Listens when firebase is Initialized
        GameManager.Instance.AchievementUpdate += OnAchievementUpdate; // Subscribe to Achievement Update Invoked by Achievement Notification

        // Subscribe to Achievement Tracker action to Listeners
        GameManager.Instance.DistanceTravelledChanged += OnDistanceTravelledChanged;
        GameManager.Instance.PunchCountChanged += OnPunchCountChanged;
        GameManager.Instance.KickCountChanged += OnKickCountChanged;
        GameManager.Instance.TotalHitsCountChanged += OnTotalHitsCountChanged;
    }

    private void OnStatsReadyForDisplay()
    {
        // Get Data From Firestore and Sync Game Setting and UI
        BasicGameSettings settings = GameManager.Instance.LoadBasicSettings();
        _settingHandler.SetSliderValues(settings.AudioVolume, settings.CameraDistance, settings.CameraSide, false);

        _distanceAchievements = GameManager.Instance.LoadInitialDistanceAchievementStates();
        _combatAchievements = GameManager.Instance.LoadInitialCombatAchievementStates();

        // Cache the achievement states to Game Manager for Notification Purposes on a single property change event
        GameManager.Instance.CachedOldRunnerAchievement = _distanceAchievements;
        GameManager.Instance.CachedOldCombatAchievement = _combatAchievements;

        // Set Achievements UI
        SetAchievementsUI(_distanceAchievements, _combatAchievements);
    }

    private void SetAchievementsUI(DistanceAchievement distanceAchievement, CombatAchievement combatAchievement)
    {
        System.Collections.Generic.Dictionary<int, bool> distanceMapping = new()
        {
            { 10, distanceAchievement.Meters10 },
            { 20, distanceAchievement.Meters20 },
            { 30, distanceAchievement.Meters30 },
            { 40, distanceAchievement.Meters40 },
            { 50, distanceAchievement.Meters50 },
            { 60, distanceAchievement.Meters60 },
            { 70, distanceAchievement.Meters70 },
            { 80, distanceAchievement.Meters80 },
            { 90, distanceAchievement.Meters90 },
            { 100, distanceAchievement.Meters100 },
            { 500, distanceAchievement.Meters500 },
            { 1000, distanceAchievement.Meters1000 }
        };

        System.Collections.Generic.Dictionary<(CombatType, int), bool> combatMapping = new()
        {
            { (CombatType.Punch, 10), combatAchievement.Punches10 },
            { (CombatType.Punch, 20), combatAchievement.Punches20 },
            { (CombatType.Punch, 50), combatAchievement.Punches50 },
            { (CombatType.Punch, 100), combatAchievement.Punches100 },
            { (CombatType.Kick, 10), combatAchievement.Kicks10 },
            { (CombatType.Kick, 20), combatAchievement.Kicks20 },
            { (CombatType.Kick, 50), combatAchievement.Kicks50 },
            { (CombatType.Kick, 100), combatAchievement.Kicks100 },
            { (CombatType.Total, 20), combatAchievement.Hits20 },
            { (CombatType.Total, 40), combatAchievement.Hits40 },
            { (CombatType.Total, 100), combatAchievement.Hits100 },
            { (CombatType.Total, 500), combatAchievement.Hits500 }
        };

        foreach (AchievementStateHandler stateHandler in _achievementStateHandlers)
        {
            // According to the state Handler item we check the types by mapping through dictionary and set values to the UI
            if (stateHandler.AchievementType == AchievementType.Runner && distanceMapping.TryGetValue(stateHandler.AchievementGoal, out bool distanceStatus)) stateHandler.SetAchievementFields(distanceStatus);
            else if (stateHandler.AchievementType == AchievementType.Fighter && combatMapping.TryGetValue((stateHandler.CombatType, stateHandler.AchievementGoal), out bool combatStatus)) stateHandler.SetAchievementFields(combatStatus);
        }
    }

    private void OnAchievementUpdate(AchievementType achievementType, CombatType combatType, int goal, bool state)
    {
        foreach (AchievementStateHandler stateHandler in _achievementStateHandlers)
        {
            // On Achievement Update in firestore, it updates the UI
            if (stateHandler.AchievementType == achievementType && stateHandler.CombatType == combatType && stateHandler.AchievementGoal == goal) stateHandler.SetAchievementFields(state);
        }
    }

    // On Each Achievement Achieved update the state on the FirestoreDB
    private void OnDistanceTravelledChanged(int value) => UpdateAchievementState(AchievementType.Runner, CombatType.None, value);

    private void OnPunchCountChanged(int value) => UpdateAchievementState(AchievementType.Fighter, CombatType.Punch, value);

    private void OnKickCountChanged(int value) => UpdateAchievementState(AchievementType.Fighter, CombatType.Kick, value);

    private void OnTotalHitsCountChanged(int value) => UpdateAchievementState(AchievementType.Fighter, CombatType.Total, value);

    private void UpdateAchievementState(AchievementType achievementType, CombatType combatType, int value)
    {
        if (achievementType == AchievementType.Runner)
        {
            DistanceAchievement distanceAchievement;
            switch (value)
            {
                case 10:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters10 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 20:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters20 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 30:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters30 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 40:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters40 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 50:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters50 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 60:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters60 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 70:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters70 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 80:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters80 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 90:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters90 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 100:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters100 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 500:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters500 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                case 1000:
                    distanceAchievement = CreateNew(GameManager.Instance.CachedOldRunnerAchievement);
                    distanceAchievement.Meters1000 = true;
                    GameManager.Instance.RunnerAchievementNotification.Invoke(distanceAchievement);
                    break;
                default:
                    break;
            }
        }
        else if (achievementType == AchievementType.Fighter)
        {
            CombatAchievement combatAchievement;
            switch (value)
            {
                case 10:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Punch) combatAchievement.Punches10 = true;
                    else if (combatType == CombatType.Kick) combatAchievement.Kicks10 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                case 20:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Punch) combatAchievement.Punches20 = true;
                    else if (combatType == CombatType.Kick) combatAchievement.Kicks20 = true;
                    else if (combatType == CombatType.Total) combatAchievement.Hits20 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                case 40:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Total) combatAchievement.Hits40 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                case 50:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Punch) combatAchievement.Punches50 = true;
                    else if (combatType == CombatType.Kick) combatAchievement.Kicks50 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                case 100:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Punch) combatAchievement.Punches100 = true;
                    else if (combatType == CombatType.Kick) combatAchievement.Kicks100 = true;
                    else if (combatType == CombatType.Total) combatAchievement.Hits100 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                case 500:
                    combatAchievement = CreateNew(GameManager.Instance.CachedOldCombatAchievement);
                    if (combatType == CombatType.Total) combatAchievement.Hits500 = true;
                    GameManager.Instance.CombatAchievementNotification.Invoke(combatAchievement);
                    break;
                default:
                    break;
            }
        }

        // The Achievement Goal is met and we can change the state
        //await GameManager.Instance.UpdateAchievementByPropertyNameToFirestore(achievementType, propertyName, true);

    }

    public DistanceAchievement CreateNew(DistanceAchievement old)
    {
        return new DistanceAchievement()
        {
            Meters10 = old.Meters10,
            Meters100 = old.Meters100,
            Meters1000 = old.Meters1000,
            Meters20 = old.Meters20,
            Meters30 = old.Meters30,
            Meters40 = old.Meters40,
            Meters50 = old.Meters50,
            Meters60 = old.Meters60,
            Meters500 = old.Meters500,
            Meters70 = old.Meters70,
            Meters80 = old.Meters80,
            Meters90 = old.Meters90
        };
    }

    public CombatAchievement CreateNew(CombatAchievement old)
    {
        return new CombatAchievement()
        {
            Punches10 = old.Punches10,
            Punches20 = old.Punches20,
            Punches50 = old.Punches50,
            Punches100 = old.Punches100,
            Kicks10 = old.Kicks10,
            Kicks20 = old.Kicks20,
            Kicks50 = old.Kicks50,
            Kicks100 = old.Kicks100,
            Hits20 = old.Hits20,
            Hits40 = old.Hits40,
            Hits100 = old.Hits100,
            Hits500 = old.Hits500
        };
    }

    // Quit the game on Yes Clicked from Quit Confirmation Panel
    public void QuitGame()
    {
        _inputs.uIControl = true;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            // Unsubscribe to all the actions
            GameManager.Instance.StatsReadyForDisplay -= OnStatsReadyForDisplay;
            GameManager.Instance.AchievementUpdate -= OnAchievementUpdate;

            GameManager.Instance.DistanceTravelledChanged -= OnDistanceTravelledChanged;
            GameManager.Instance.PunchCountChanged -= OnPunchCountChanged;
            GameManager.Instance.KickCountChanged -= OnKickCountChanged;
            GameManager.Instance.TotalHitsCountChanged -= OnTotalHitsCountChanged;
        }
    }
}

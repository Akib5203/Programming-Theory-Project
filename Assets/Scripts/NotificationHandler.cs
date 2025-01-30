using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    [SerializeField] private GameObject _savedNotification;
    [SerializeField] private AchievementNotificationHandler _achievementNotificationHandler;

    [Header("Achievement Notification Animation")]
    [SerializeField] private AnimationClip _achievementNotificationClip;

    private float _notificationClipLength;
    private bool _isNotificationDisplaying;

    private System.Collections.Generic.Queue<(AchievementType, string)> _notificationQueue = new();

    private void OnEnable()
    {
        _notificationClipLength = _achievementNotificationClip.length; // For Queing Purpose

        // Subscribe to actions
        GameManager.Instance.SavedNotification += OnSavedNotification;
        GameManager.Instance.RunnerAchievementNotification += OnRunnerAchievementNotification;
        GameManager.Instance.CombatAchievementNotification += OnCombatAchievementNotification;
    }

    private void OnSavedNotification() => _savedNotification.SetActive(true); // Display Saved Notification

    private void OnRunnerAchievementNotification(DistanceAchievement distanceAchievement)
    {
        if (GameManager.Instance.CachedOldRunnerAchievement != null)
        {
            (int goal, string name, bool state) = DetectRunnerChanges(GameManager.Instance.CachedOldRunnerAchievement, distanceAchievement);

            if (state) // if achievement completed then display Notification
            {
                // By using Queue and Coroutine we can handle notification one after the other without overlapping
                _notificationQueue.Enqueue((AchievementType.Runner, name));

                if (!_isNotificationDisplaying) StartCoroutine(DisplayNotifications());

                 GameManager.Instance.AchievementUpdate.Invoke(AchievementType.Runner, CombatType.None, goal, state); // This statement invokes another method that is responsible for UI value change
            }
        }

        GameManager.Instance.CachedOldRunnerAchievement = distanceAchievement; // Set Cached Achievement with new one
    }

    private void OnCombatAchievementNotification(CombatAchievement combatAchievement)
    {
        if (GameManager.Instance.CachedOldCombatAchievement != null)
        {
            (CombatType combatType, int goal, string name, bool state) = DetectCombatChanges(GameManager.Instance.CachedOldCombatAchievement, combatAchievement);

            if (state) // if achievement completed then display Notification
            {
                _notificationQueue.Enqueue((AchievementType.Fighter, name));

                if (!_isNotificationDisplaying) StartCoroutine(DisplayNotifications());

                GameManager.Instance.AchievementUpdate.Invoke(AchievementType.Fighter, combatType, goal, state); // This statement invokes another method that is responsible for UI value change
            }
        }

        GameManager.Instance.CachedOldCombatAchievement = combatAchievement; // Set Cached Achievement with new one
    }

    private (int, string, bool) DetectRunnerChanges(DistanceAchievement oldState, DistanceAchievement newState)
    {
        // Compare each property and return the first tuple of achievement goal integer value and string name and bool value that don't match value
        if (oldState.Meters10 != newState.Meters10) return (10, "10 Meters", newState.Meters10);
        if (oldState.Meters20 != newState.Meters20) return (20, "20 Meters", newState.Meters20);
        if (oldState.Meters30 != newState.Meters30) return (30, "30 Meters", newState.Meters30);
        if (oldState.Meters40 != newState.Meters40) return (40, "40 Meters", newState.Meters40);
        if (oldState.Meters50 != newState.Meters50) return (50, "50 Meters", newState.Meters50);
        if (oldState.Meters60 != newState.Meters60) return (60, "60 Meters", newState.Meters60);
        if (oldState.Meters70 != newState.Meters70) return (70, "70 Meters", newState.Meters70);
        if (oldState.Meters80 != newState.Meters80) return (80, "80 Meters", newState.Meters80);
        if (oldState.Meters90 != newState.Meters90) return (90, "90 Meters", newState.Meters90);
        if (oldState.Meters100 != newState.Meters100) return (100, "100 Meters", newState.Meters100);
        if (oldState.Meters500 != newState.Meters500) return (500, "500 Meters", newState.Meters500);
        if (oldState.Meters1000 != newState.Meters1000) return (1000, "1000 Meters", newState.Meters1000);
        return (0, string.Empty, false);
    }
    
    private (CombatType, int, string, bool) DetectCombatChanges(CombatAchievement oldState, CombatAchievement newState)
    {
        // Compare each property and return the first tuple of Combat type value and achievement goal integer value and string name and bool value that don't match value
        if (oldState.Punches10 != newState.Punches10) return (CombatType.Punch, 10, "10 Punches", newState.Punches10);
        if (oldState.Punches20 != newState.Punches20) return (CombatType.Punch, 20, "20 Punches", newState.Punches20);
        if (oldState.Punches50 != newState.Punches50) return (CombatType.Punch, 50, "50 Punches", newState.Punches50);
        if (oldState.Punches100 != newState.Punches100) return (CombatType.Punch, 100, "100 Punches", newState.Punches100);
        if (oldState.Kicks10 != newState.Kicks10) return (CombatType.Kick, 10, "10 Kicks", newState.Kicks10);
        if (oldState.Kicks20 != newState.Kicks20) return (CombatType.Kick, 20, "20 Kicks", newState.Kicks20);
        if (oldState.Kicks50 != newState.Kicks50) return (CombatType.Kick, 50, "50 Kicks", newState.Kicks50);
        if (oldState.Kicks100 != newState.Kicks100) return (CombatType.Kick, 100, "100 Kicks", newState.Kicks100);
        if (oldState.Hits20 != newState.Hits20) return (CombatType.Total, 20, "20 Total Hits", newState.Hits20);
        if (oldState.Hits40 != newState.Hits40) return (CombatType.Total, 40, "40 Total Hits", newState.Hits40);
        if (oldState.Hits100 != newState.Hits100) return (CombatType.Total, 100, "100 Total Hits", newState.Hits100);
        if (oldState.Hits500 != newState.Hits500) return (CombatType.Total, 500, "500 Total Hits", newState.Hits500);
        return (CombatType.None, 0, string.Empty, false);
    }

    private System.Collections.IEnumerator DisplayNotifications()
    {
        _isNotificationDisplaying = true;

        while (_notificationQueue.Count > 0)
        {
            (AchievementType achievementType, string name) = _notificationQueue.Dequeue();

            // Update UI elements
            _achievementNotificationHandler.SetAchievementNotificationValues(achievementType, name);
            _achievementNotificationHandler.gameObject.SetActive(true);

            // Show the notification for the animation duration
            yield return new WaitForSeconds(_notificationClipLength);

            // small delay between notifications
            yield return new WaitForSeconds(0.2f);
        }
        _isNotificationDisplaying = false;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            // Unsubscribe
            GameManager.Instance.SavedNotification -= OnSavedNotification;
            GameManager.Instance.RunnerAchievementNotification -= OnRunnerAchievementNotification;
            GameManager.Instance.CombatAchievementNotification -= OnCombatAchievementNotification;
        }
    }
}

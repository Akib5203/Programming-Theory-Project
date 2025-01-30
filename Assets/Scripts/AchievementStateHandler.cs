using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementStateHandler : MonoBehaviour
{
    public AchievementType AchievementType;
    public CombatType CombatType;
    public int AchievementGoal;

    [Space(10)]
    [SerializeField] private Status _currentStatus;

    [SerializeField] private Image _completionImage;
    [SerializeField] private TMP_Text _completionText;

    [Header("Text Color")]
    [SerializeField] private Color _redColor;
    [SerializeField] private Color _greenColor;

    [Header("Status Sprite")]
    [SerializeField] private Sprite _completedSprite;
    [SerializeField] private Sprite _notCompletedSprite;

    public void SetAchievementFields(bool achievementStatus)
    {
        _currentStatus = achievementStatus ? Status.Completed : Status.NotCompleted; // Set Current Status

        // Using Switch Statement return a tuple of string, color and sprite
        (string text, Color color, Sprite sprite) = _currentStatus switch
        {
            Status.Completed => ("Completed", _greenColor, _completedSprite),
            Status.NotCompleted => ("Not Completed", _redColor, _notCompletedSprite),
            _ => ("Not Completed", _redColor, _notCompletedSprite)
        };

        // Sets the value according to the tuple above
        _completionImage.sprite = sprite;
        _completionText.text = text;
        _completionText.color = color;
    }
}

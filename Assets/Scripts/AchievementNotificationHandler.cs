using UnityEngine;

public class AchievementNotificationHandler : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image _achievementImage;
    [SerializeField] private TMPro.TMP_Text _achievementText;

    [Header("Sprites")]
    [SerializeField] private Sprite _runnerSprite;
    [SerializeField] private Sprite _combatSprite;

    public void SetAchievementNotificationValues(AchievementType achievementType, string text)
    {
        _achievementText.text = text + " Completed"; // Sets the text UI to the text value

        _achievementImage.sprite = achievementType switch // Based on Achievement Type the sprite is set
        {
            AchievementType.Runner => _runnerSprite,
            AchievementType.Fighter => _combatSprite,
            _ => null
        };
    }

    //Set Gameobject false on Animation end. This method is used on the last frame of animation of keyframe event.
    public void OnAchievementAnimationEnd() => gameObject.SetActive(false);
}

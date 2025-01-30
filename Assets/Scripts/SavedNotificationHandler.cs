using UnityEngine;

public class SavedNotificationHandler : MonoBehaviour
{
    //Set Gameobject false on Animation end. This method is used on the last frame of animation of keyframe event.
    public void OnSavedAnimationEnd() => gameObject.SetActive(false);
}

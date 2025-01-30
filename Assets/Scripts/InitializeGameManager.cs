using UnityEngine;

public class InitializeGameManager : MonoBehaviour
{
    private void Awake() => GameManager.Instance.StartGameManager();
}

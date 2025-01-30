using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroHandler : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _startGameButton.onClick.AddListener(OnStartGameClicked);
        _quitGameButton.onClick.AddListener(OnQuitGameClicked);
    }

    private void OnStartGameClicked() => SceneManager.LoadSceneAsync(1);

    private void OnQuitGameClicked() => Application.Quit();

    private void OnDisable()
    {
        _startGameButton.onClick.RemoveListener(OnStartGameClicked);
        _quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
    }
}

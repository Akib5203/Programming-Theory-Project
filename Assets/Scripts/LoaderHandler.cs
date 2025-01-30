using UnityEngine;

public class LoaderHandler : MonoBehaviour
{
    [SerializeField] private GameObject _inputCover;
    [SerializeField] private GameObject _loaderImageObj;

    private void OnEnable() => GameManager.Instance.Loading += ToggleLoader;

    private void ToggleLoader(bool toggle)
    {
        // Display the loader and cover (cover that covers all the inputs in the UI from being clicked) based on the boolean toggle value
        _inputCover.SetActive(toggle);
        _loaderImageObj.SetActive(toggle);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.Loading -= ToggleLoader;
    }
}

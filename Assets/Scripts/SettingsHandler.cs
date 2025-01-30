using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [Header("Virtual Camera")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _virtualCamera;

    [Header("Third Person Controller")]
    [SerializeField] private StarterAssets.ThirdPersonController _thirdPersonController;

    [Header("Audio Volume")]
    [SerializeField] private Slider _audioSlider;
    [SerializeField] private TMP_Text _audioValue;

    [Header("Camera Distance")]
    [SerializeField] private Slider _cameraDistanceSlider;
    [SerializeField] private TMP_Text _cameraDistanceValue;

    [Header("Camera Side")]
    [SerializeField] private Slider _cameraSideSlider;
    [SerializeField] private TMP_Text _cameraSideValue;

    [Header("Save Button")]
    [SerializeField] private Button _saveButton;

    private Cinemachine.Cinemachine3rdPersonFollow _framing3rdPersonFollow;

    private void OnEnable()
    {
        // Get the Framing 3rd Person Follow reference from the Cinemachine Virtual camera
        if (_framing3rdPersonFollow == null) _framing3rdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine.Cinemachine3rdPersonFollow>();

        // Subscribed to OnChange event listeners of Sliders
        _audioSlider.onValueChanged.AddListener(OnAudioSliderValueChanged);
        _cameraDistanceSlider.onValueChanged.AddListener(OnCameraDistanceSliderValueChanged);
        _cameraSideSlider.onValueChanged.AddListener(OnCameraSideSliderValueChanged);

        // Subscribed to button click event
        _saveButton.onClick.AddListener(OnSave);
    }

    public void SetSliderValues(float audioSliderValue, float cameraDistanceSliderValue, float cameraSideSliderValue, bool callingFromThisClass)
    {
        // Truncate the float values upto only 3 decimal points
        audioSliderValue = TruncateFloatValue(audioSliderValue, 3);
        cameraDistanceSliderValue = TruncateFloatValue(cameraDistanceSliderValue, 3);
        cameraSideSliderValue = TruncateFloatValue(cameraSideSliderValue, 3);

        // Set Slider values
        _audioSlider.value = audioSliderValue;
        _cameraDistanceSlider.value = cameraDistanceSliderValue;
        _cameraSideSlider.value = cameraSideSliderValue;

        if (!callingFromThisClass) // If we are calling from this class then we don't need to perform the codes below as the Silder change event will handle the values
        {
            // Update the UI Text values
            _audioValue.text = audioSliderValue.ToString();
            _cameraDistanceValue.text = cameraDistanceSliderValue.ToString();
            _cameraSideValue.text = cameraSideSliderValue.ToString();

            _thirdPersonController.FootstepAudioVolume = audioSliderValue; // Set Audio Volume

            // Get Framing 3rd Person Follow reference if not available before
            if (_framing3rdPersonFollow == null) _framing3rdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine.Cinemachine3rdPersonFollow>();
            _framing3rdPersonFollow.CameraDistance = cameraDistanceSliderValue; // Set the camera distance
            _framing3rdPersonFollow.CameraSide = cameraSideSliderValue; // Set the camera side value
        }
    }

    private void OnAudioSliderValueChanged(float value)
    {
        value = TruncateFloatValue(value, 3);
        _thirdPersonController.FootstepAudioVolume = value;
        _audioValue.text = value.ToString();
    }
    private void OnCameraDistanceSliderValueChanged(float value)
    {
        value = TruncateFloatValue(value, 3);
        _framing3rdPersonFollow.CameraDistance = value;
        _cameraDistanceValue.text = value.ToString();
    }
    private void OnCameraSideSliderValueChanged(float value)
    {
        value = TruncateFloatValue(value, 3);
        _framing3rdPersonFollow.CameraSide = value;
        _cameraSideValue.text = value.ToString();
    }

    private void OnSave()
    {
        // Create an object of BasicSettings and set the new values
        BasicGameSettings basicGameSettings = new() 
        { 
            AudioVolume = TruncateFloatValue(_audioSlider.value, 3),
            CameraDistance = TruncateFloatValue(_cameraDistanceSlider.value, 3),
            CameraSide = TruncateFloatValue(_cameraSideSlider.value, 3)
        };
        GameManager.Instance.SaveBasicSettings(basicGameSettings);
    }

    private float TruncateFloatValue(float value, int decimalNumber) => (float)System.Math.Round(value, decimalNumber);

    private void OnDisable()
    {
        // Unsubscribed to OnChange event listeners of Sliders
        _audioSlider.onValueChanged.RemoveListener(OnAudioSliderValueChanged);
        _cameraDistanceSlider.onValueChanged.RemoveListener(OnCameraDistanceSliderValueChanged);
        _cameraSideSlider.onValueChanged.RemoveListener(OnCameraSideSliderValueChanged);

        // Unsubscribed to button click event
        _saveButton.onClick.RemoveListener(OnSave);
    }
}

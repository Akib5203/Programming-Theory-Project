using UnityEngine;

[System.Serializable]
public class BasicGameSettings
{
    [SerializeField] private float audioVolume;
    [SerializeField] private float cameraDistance;
    [SerializeField] private float cameraSide;

    public float AudioVolume { get => audioVolume; set => audioVolume = value; }
    public float CameraDistance { get => cameraDistance; set => cameraDistance = value; }
    public float CameraSide { get => cameraSide; set => cameraSide = value; }
}

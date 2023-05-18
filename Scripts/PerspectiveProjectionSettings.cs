using UnityEngine;

[CreateAssetMenu(fileName = "PerspectiveProjectionSettings", menuName = "ScriptableObjects/PerspectiveProjectionSettings", order = 1)]
public class PerspectiveProjectionSettings : ScriptableObject
{
    public GameObject cameraOrigin;
    public Camera trackedCamera;
    public Camera displayCamera;
    public Vector2Int resolution;
    public float screenHeight;
    public float scaleFactor;
}

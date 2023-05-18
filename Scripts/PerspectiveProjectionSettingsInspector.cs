using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerspectiveProjectionSettings))]
public class PerspectiveProjectionSettingsInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        var settings = (PerspectiveProjectionSettings)target;

        var cameraOriginField = new ObjectField("Camera Origin") { objectType = typeof(GameObject), value = settings.cameraOrigin };
        cameraOriginField.RegisterValueChangedCallback(evt => { settings.cameraOrigin = (GameObject)evt.newValue; });

        var trackedCameraField = new ObjectField("Tracked Camera") { objectType = typeof(Camera), value = settings.trackedCamera };
        trackedCameraField.RegisterValueChangedCallback(evt => { settings.trackedCamera = (Camera)evt.newValue; });

        var displayCameraField = new ObjectField("Display Camera") { objectType = typeof(Camera), value = settings.displayCamera };
        displayCameraField.RegisterValueChangedCallback(evt => { settings.displayCamera = (Camera)evt.newValue; });

        var resolutionField = new Vector2IntField("Resolution") { value = settings.resolution };
        resolutionField.RegisterValueChangedCallback(evt => { settings.resolution = evt.newValue; });

        var screenHeightField = new FloatField("Screen Height") { value = settings.screenHeight };
        screenHeightField.RegisterValueChangedCallback(evt => { settings.screenHeight = evt.newValue; });

        var scaleFactorField = new FloatField("Scale Factor") { value = settings.scaleFactor };
        scaleFactorField.RegisterValueChangedCallback(evt => { settings.scaleFactor = evt.newValue; });

        root.Add(cameraOriginField);
        root.Add(trackedCameraField);
        root.Add(displayCameraField);
        root.Add(resolutionField);
        root.Add(screenHeightField);
        root.Add(scaleFactorField);

        return root;
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackController))]
public class TrackControllerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TrackController trackController = (TrackController)target;

        if (GUILayout.Button("Generate Track")) {
            trackController.GenerateTrack();
        }
    }
}

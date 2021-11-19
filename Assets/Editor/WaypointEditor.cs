using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        Handles.PositionHandle(Vector3.zero, Quaternion.identity);
    }
}

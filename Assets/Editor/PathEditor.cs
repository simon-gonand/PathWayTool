using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    SerializedProperty waypointsList;

    private void OnEnable()
    {
        waypointsList = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(waypointsList);

        if (GUILayout.Button("Create Point"))
        {
            GameObject obj = new GameObject("Waypoint " + waypointsList.arraySize);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.AddComponent<Waypoint>();
            // Set Waypoint value
            waypointsList.InsertArrayElementAtIndex(waypointsList.arraySize );
            EditorUtility.SetDirty(obj);
            Undo.RegisterCreatedObjectUndo(obj, "Create waypoint");
        }

        serializedObject.ApplyModifiedProperties();
    }
}

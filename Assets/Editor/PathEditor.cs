using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    SerializedProperty waypointsList;
    Path pathScript;
        
    private void OnEnable()
    {
        waypointsList = serializedObject.FindProperty("waypoints");
        pathScript = target as Path;
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
            obj.transform.SetParent(pathScript.transform);
            Waypoint wp = obj.AddComponent<Waypoint>();
            wp.parentPath = pathScript;
            // Set Waypoint value
            waypointsList.InsertArrayElementAtIndex(waypointsList.arraySize);
            SerializedProperty sp = waypointsList.GetArrayElementAtIndex(waypointsList.arraySize - 1);
            sp.objectReferenceValue = wp;
            EditorUtility.SetDirty(obj);
            Undo.RegisterCreatedObjectUndo(obj, "Create waypoint");
        }

        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    SerializedProperty waypointsList;
    SerializedProperty linksList;
    Path pathScript;
        
    private void OnEnable()
    {
        waypointsList = serializedObject.FindProperty("waypoints");
        linksList = serializedObject.FindProperty("links");
        pathScript = target as Path;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(waypointsList);
        EditorGUILayout.PropertyField(linksList);
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
            if (waypointsList.arraySize > 1)
            {
                for (int i = 1; i < waypointsList.arraySize; ++i)
                {
                    Waypoint start = waypointsList.GetArrayElementAtIndex(i - 1).objectReferenceValue as Waypoint;
                    Waypoint end = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
                    GameObject go = new GameObject("Link");
                    Link link = go.AddComponent<Link>();
                    link.start = start;
                    link.end = end;
                    // Manage list
                    go.transform.SetParent(pathScript.transform);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}

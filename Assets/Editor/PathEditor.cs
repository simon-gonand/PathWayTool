using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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

    private void CreateWaypoint()
    {
        GameObject obj = new GameObject("Waypoint " + waypointsList.arraySize);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(pathScript.transform);
        Waypoint wp = obj.AddComponent<Waypoint>();
        wp.parentPath = pathScript;
        // Set Waypoint value
        waypointsList.InsertArrayElementAtIndex(waypointsList.arraySize);
        waypointsList.GetArrayElementAtIndex(waypointsList.arraySize - 1).objectReferenceValue = wp;
        EditorUtility.SetDirty(obj);
        Undo.RegisterCreatedObjectUndo(obj, "Create waypoint");
    }

    private void CalculateLinks()
    {
        if (linksList.arraySize <= 0) return;

        for (int i = 0; i < linksList.arraySize; ++i)
        {
            Link link = linksList.GetArrayElementAtIndex(i).objectReferenceValue as Link;
            NavMeshPath linkPath = new NavMeshPath();
            if(NavMesh.CalculatePath(link.start.transform.position, link.end.transform.position, NavMesh.AllAreas, linkPath))
                if (link.pathPoints.Count > 0)
                    link.pathPoints.Clear();
            for (int j = 0; j < linkPath.corners.Length; ++j)
            {
                link.pathPoints.Add(linkPath.corners[j]);
            }
        }
    }

    private void CreateLinks()
    {
        for (int i = 1; i < waypointsList.arraySize; ++i)
        {
            Waypoint start = waypointsList.GetArrayElementAtIndex(i - 1).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;

            // Avoid to duplicate links
            bool linkAlreadyExist = false;
            for (int j = 0; j < linksList.arraySize; ++j)
            {
                Link l = linksList.GetArrayElementAtIndex(j).objectReferenceValue as Link;
                if (l.Equals(start, end))
                {
                    linkAlreadyExist = true;
                    break;
                }
            }
            if (linkAlreadyExist) continue;

            GameObject go = new GameObject("Link");
            Link link = go.AddComponent<Link>();
            link.start = start;
            link.end = end;
            linksList.InsertArrayElementAtIndex(linksList.arraySize);
            linksList.GetArrayElementAtIndex(linksList.arraySize - 1).objectReferenceValue = link;
            link.parentPath = pathScript;
            go.transform.SetParent(pathScript.transform);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(waypointsList);
        EditorGUILayout.PropertyField(linksList);
        if (GUILayout.Button("Create Point"))
        {
            CreateWaypoint();
            if (waypointsList.arraySize > 1)
                CreateLinks();
        }

        if (GUILayout.Button("Calculate Path"))
            CalculateLinks();
        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    SerializedProperty waypointsList;
    SerializedProperty linksList;

    ReorderableList waypointsRList;
    ReorderableList linksRList;

    Waypoint selectedWaypoint;
    int selectedWaypointIndex;
    Link selectedLink;

    Path pathScript;
        
    private void OnEnable()
    {
        waypointsList = serializedObject.FindProperty("waypoints");
        linksList = serializedObject.FindProperty("links");
        pathScript = target as Path;

        waypointsRList = new ReorderableList(serializedObject, waypointsList);
        waypointsRList.drawHeaderCallback += HeaderWaypointCallback;
        waypointsRList.onSelectCallback += SelectWaypointCallback;
        waypointsRList.drawElementCallback += ElementWaypointCallback;

        linksRList = new ReorderableList(serializedObject, linksList);
        linksRList.drawHeaderCallback += HeaderLinkCallback;
        linksRList.onSelectCallback += SelectLinkCallback;
        linksRList.drawElementCallback += ElementLinkCallback;
    }

    #region waypoints reorderable list
    private void HeaderWaypointCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Waypoints");
    }

    private void SelectWaypointCallback(ReorderableList rList)
    {
        SerializedProperty sp = waypointsList.GetArrayElementAtIndex(rList.index);
        selectedWaypoint = sp.objectReferenceValue as Waypoint;
        selectedWaypointIndex = rList.index;
        selectedLink = null;
    }

    private void ElementWaypointCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        Waypoint waypoint = (waypointsList.GetArrayElementAtIndex(index).objectReferenceValue as Waypoint);
        EditorGUI.BeginChangeCheck();
        waypoint.transform.position = EditorGUI.Vector3Field(rect, "Waypoint " + index, waypoint.transform.position);
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(waypoint.gameObject);
    }
    #endregion

    #region links reorderable list
    private void HeaderLinkCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Links");
    }

    private void SelectLinkCallback(ReorderableList rList)
    {
        SerializedProperty sp = linksList.GetArrayElementAtIndex(rList.index);
        selectedLink = sp.objectReferenceValue as Link;
        selectedWaypoint = null;
    }
    private void ElementLinkCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        EditorGUI.LabelField(rect, index + " - " + (index + 1));
    }
    #endregion

    private void CreateWaypoint()
    {
        GameObject obj = new GameObject("Waypoint " + waypointsList.arraySize);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(pathScript.transform);
        obj.hideFlags = HideFlags.HideInHierarchy;
        Waypoint wp = obj.AddComponent<Waypoint>();

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
            if (NavMesh.CalculatePath(link.start.transform.position, link.end.transform.position, NavMesh.AllAreas, linkPath))
            {
                if (link.pathPoints.Count > 0)
                    link.pathPoints.Clear();
            }
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
            go.transform.SetParent(pathScript.transform);
            go.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //EditorGUILayout.PropertyField(waypointsList);
        waypointsRList.DoLayoutList();
        linksRList.DoLayoutList();
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

    private void OnSceneGUI()
    {
        Tools.current = Tool.None;
        for (int i = 0; i < waypointsList.arraySize; ++i)
        {
            Waypoint waypoint = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            Handles.DrawSolidDisc(waypoint.transform.position, Vector3.up, 0.3f);
        }
        Handles.color = Color.red;
        for (int i = 0; i < linksList.arraySize; ++i)
        {
            Link link = linksList.GetArrayElementAtIndex(i).objectReferenceValue as Link;
            for (int j = 1; j < link.pathPoints.Count; ++j)
                Handles.DrawLine(link.pathPoints[j - 1], link.pathPoints[j]);
        }
        if (selectedWaypoint) {
            Undo.RecordObject(selectedWaypoint.transform, "Move Waypoints");
            selectedWaypoint.transform.position = Handles.PositionHandle(
                selectedWaypoint.transform.position,
                selectedWaypoint.transform.rotation);
            EditorUtility.SetDirty(selectedWaypoint.transform);
            if (linksList.arraySize > 0)
            {
                if (selectedWaypointIndex < linksList.arraySize)
                {
                    Link link = linksList.GetArrayElementAtIndex(selectedWaypointIndex).objectReferenceValue as Link;
                    if (link.pathPoints.Count == 0) return;
                    link.pathPoints[0] = selectedWaypoint.transform.position;
                }
                if (selectedWaypointIndex > 0)
                {
                    Link link = linksList.GetArrayElementAtIndex(selectedWaypointIndex - 1).objectReferenceValue as Link;
                    if (link.pathPoints.Count == 0) return;
                    link.pathPoints[link.pathPoints.Count - 1] = selectedWaypoint.transform.position;
                }
            }
        }

        if (selectedLink && selectedLink.pathPoints.Count != 0)
        {
            
            for (int i = 0; i < selectedLink.pathPoints.Count; ++i)
            {
                Undo.RecordObject(selectedLink, "Move Path points");
                selectedLink.pathPoints[i] = Handles.PositionHandle(
                    selectedLink.pathPoints[i],
                    selectedLink.transform.rotation);
                EditorUtility.SetDirty(selectedLink);
            }

            selectedLink.start.transform.position = selectedLink.pathPoints[0];

            selectedLink.end.transform.position = selectedLink.pathPoints[selectedLink.pathPoints.Count - 1];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
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
    int selectedLinkIndex;

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
        waypointsRList.onAddCallback += AddWaypointCallback;
        waypointsRList.onRemoveCallback += RemoveWaypointCallback;
        waypointsRList.draggable = false;

        linksRList = new ReorderableList(serializedObject, linksList);
        linksRList.drawHeaderCallback += HeaderLinkCallback;
        linksRList.onSelectCallback += SelectLinkCallback;
        linksRList.drawElementCallback += ElementLinkCallback;
        linksRList.draggable = false;
        linksRList.displayAdd = false;
        linksRList.displayRemove = false;
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
        Rect leftRect = new Rect(rect.x, rect.y, rect.width / 2, rect.height);
        Rect labelRect = new Rect(rect.x, rect.y, leftRect.width - leftRect.width / 5, rect.height);
        EditorGUI.LabelField(labelRect, "Waypoint " + index);

        Rect buttonRect = new Rect(labelRect.x + labelRect.width, rect.y, leftRect.width / 5, rect.height);
        if (GUI.Button(buttonRect, "+"))
        {
            GenericMenu addMenu = new GenericMenu();

            addMenu.AddItem(new GUIContent("Add waypoint above"), false, CreateWaypointAndLinks, index);
            addMenu.AddItem(new GUIContent("Add waypoint below"), false, CreateWaypointAndLinks, index + 1);

            addMenu.ShowAsContext();
        }

        Rect rightRect = new Rect(leftRect.x + leftRect.width, rect.y, rect.width / 2, rect.height);
        waypoint.transform.position = EditorGUI.Vector3Field(rightRect, GUIContent.none, waypoint.transform.position);
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(waypoint.gameObject);
    }

    private void AddWaypointCallback(ReorderableList rlist)
    {
        CreateWaypoint(waypointsList.arraySize);
        if (waypointsList.arraySize > 1)
            CreateLinks();
        waypointsRList.index = waypointsList.arraySize - 1;
        waypointsRList.onSelectCallback(waypointsRList);
    }

    private void RemoveWaypointCallback(ReorderableList rlist)
    {
        selectedWaypoint = null;
        if (linksList.arraySize == 0)
        {
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }
        if (rlist.index == 0)
        {
            linksList.DeleteArrayElementAtIndex(0);
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }
        if (rlist.index == linksList.arraySize)
        {
            linksList.DeleteArrayElementAtIndex(linksList.arraySize - 1);
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }

        linksList.DeleteArrayElementAtIndex(rlist.index - 1);
        linksList.DeleteArrayElementAtIndex(rlist.index - 1);
        Waypoint start = waypointsList.GetArrayElementAtIndex(rlist.index - 1).objectReferenceValue as Waypoint;
        Waypoint end = waypointsList.GetArrayElementAtIndex(rlist.index + 1).objectReferenceValue as Waypoint;
        CreateLink(start, end, rlist.index - 1);
        waypointsList.DeleteArrayElementAtIndex(rlist.index);
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
        selectedLinkIndex = rList.index;
        selectedWaypoint = null;
    }
    private void ElementLinkCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        EditorGUI.LabelField(rect, index + " - " + (index + 1));
    }
    #endregion

    private void CreateWaypointAndLinks(object index)
    {
        serializedObject.Update();
        int i = (int)index;
        Debug.Log(i);
        CreateWaypoint(i);
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        if (i > 0 && i < waypointsList.arraySize - 1 && linksList.arraySize > 0)
        {
            linksList.DeleteArrayElementAtIndex(i - 1);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        if (i > 0)
        {
            Waypoint start = waypointsList.GetArrayElementAtIndex(i - 1).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            if (linksList.arraySize == 0)
            {
                Link link = CreateLink(start, end, 0);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Link link = CreateLink(start, end, i - 1);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        if (i  < waypointsList.arraySize && waypointsList.arraySize > linksList.arraySize + 1)
        {
            Waypoint start = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i + 1).objectReferenceValue as Waypoint;
            if (linksList.arraySize == 0)
            {
                Link link = CreateLink(start, end, 0);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Link link = CreateLink(start, end, i);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateWaypoint(int index)
    {
        int i = (int)index;
        GameObject obj = new GameObject();
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(pathScript.transform);
        obj.hideFlags = HideFlags.HideInHierarchy;
        Waypoint wp = obj.AddComponent<Waypoint>();

        
        waypointsList.InsertArrayElementAtIndex(i);
        waypointsList.GetArrayElementAtIndex(i).objectReferenceValue = wp;
        EditorUtility.SetDirty(obj);
        Undo.RegisterCreatedObjectUndo(obj, "Create waypoint");
    }

    private void CalculateLinks()
    {
        if (linksList.arraySize <= 0) return;

        for (int i = 0; i < linksList.arraySize; ++i)
        {
            Link link = linksList.GetArrayElementAtIndex(i).objectReferenceValue as Link;
            CalculateLink(link);
        }
    }

    private void CalculateLink(Link link)
    {
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

    private Link CreateLink(Waypoint start, Waypoint end, int index)
    {
        GameObject go = new GameObject("Link");
        Link link = go.AddComponent<Link>();
        link.start = start;
        link.end = end;
        linksList.InsertArrayElementAtIndex(index);
        linksList.GetArrayElementAtIndex(index).objectReferenceValue = link;
        go.transform.SetParent(pathScript.transform);
        go.hideFlags = HideFlags.HideInHierarchy;
        return link;
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

            CreateLink(start, end, linksList.arraySize);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //EditorGUILayout.PropertyField(waypointsList);
        waypointsRList.DoLayoutList();
        linksRList.DoLayoutList();
        
        if (GUILayout.Button("Calculate Path"))
            CalculateLinks();
        if (GUILayout.Button("Calculate Selected Link"))
        {
            if (!selectedLink)
            {
                Debug.LogWarning("No link has been selected");
                return;
            }
            CalculateLink(selectedLink);
        }
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
            if (selectedLinkIndex > 0)
            {
                Link previousLink = linksList.GetArrayElementAtIndex(selectedLinkIndex - 1).objectReferenceValue as Link;
                previousLink.pathPoints[previousLink.pathPoints.Count - 1] = selectedLink.pathPoints[0];
                previousLink.end.transform.position = selectedLink.pathPoints[0];
            }

            selectedLink.end.transform.position = selectedLink.pathPoints[selectedLink.pathPoints.Count - 1];
            if (selectedLinkIndex < linksList.arraySize - 1)
            {
                Link nextLink = linksList.GetArrayElementAtIndex(selectedLinkIndex + 1).objectReferenceValue as Link;
                nextLink.pathPoints[0] = selectedLink.end.transform.position;
                nextLink.start.transform.position = selectedLink.pathPoints[0];
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathWayTool
{
    [MenuItem("Tools/Home Tool/Create New Path #P")]
    public static void CreatePath()
    {
        GameObject obj = new GameObject("Path");
        obj.AddComponent<Path>();
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        EditorUtility.SetDirty(obj);
        Undo.RegisterCreatedObjectUndo(obj, "Create path");
    }
}

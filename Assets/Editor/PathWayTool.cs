using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathWayTool : EditorWindow
{
    [MenuItem("Tools/Home Tool/Path Way Tool #P")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PathWayTool));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Path"))
        {
            if (Object.FindObjectOfType(typeof(Path)))
            {
                Debug.Log("There's already a path in the scene");
                return;
            }
            GameObject obj = new GameObject("Path");
            obj.AddComponent<Path>();
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            EditorUtility.SetDirty(obj);
            Undo.RegisterCreatedObjectUndo(obj, "Create path");
        }
    }
}

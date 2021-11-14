using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathWayTool : EditorWindow
{
    [MenuItem("Tools/Path Way Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PathWayTool));
    }
}

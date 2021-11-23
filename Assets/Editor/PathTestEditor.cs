using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (PathTest))]
public class PathTestEditor : Editor
{
    PathTest path;

    private void OnEnable()
    {
        path = target as PathTest;
    }

    private void OnSceneGUI()
    {
        for (int i = 0; i < path.points.Count; ++i)
        {
            if (path.points.Count < 2) break;
            Handles.DrawSolidDisc(path.points[i], Vector3.up, 0.5f);
            if (i == path.points.Count - 1) continue;
            Vector3 previousPoint;
            Vector3 nextPoint;
            if (i == 0 && i < path.points.Count - 1)
            {
                nextPoint = path.points[i + 1];
                Vector3 nextDist = nextPoint - path.points[i];
                previousPoint = path.points[i] - nextDist;
            }
            else
            {
                previousPoint = path.points[i - 1];
                nextPoint = path.points[i + 1];
            }
            Vector3 secNextPoint;
            if (i == path.points.Count - 2)
            {
                Vector3 nextDist = nextPoint - path.points[i];
                secNextPoint = nextPoint + nextDist;
            }
            else
                secNextPoint = path.points[i + 2];

            if (path.anchors.Count != path.points.Count + 1)
            {
                Vector3 startTan = nextPoint - previousPoint;
                Vector3 endTan = path.points[i] - secNextPoint;
                Vector3 startAnchor = path.points[i] + startTan * path.strenght;
                Vector3 endAnchor = nextPoint + endTan * path.strenght;
                path.anchors.Add(startAnchor);
                path.anchors.Add(endAnchor);
            }

            Undo.RecordObject(path, "Modify Path");
            EditorGUI.BeginChangeCheck();
            path.anchors[i * 2] = Handles.Slider2D(path.anchors[i * 2], Vector3.up, Vector3.right, Vector3.forward, 1.0f, Handles.DotHandleCap, Handles.SnapValue(1.0f, 1.0f));
            if (EditorGUI.EndChangeCheck())
            {
                if (i > 0)
                {
                    Vector3 endDist = path.anchors[i * 2 - 1] - path.points[i];
                    Vector3 startDist = path.anchors[i * 2] - path.points[i];
                    Vector3 offset = (endDist - startDist).normalized;
                    path.anchors[i * 2 - 1] = path.points[i] - (startDist + offset);
                }
            }
            EditorGUI.BeginChangeCheck();
            path.anchors[i * 2 + 1] = Handles.Slider2D(path.anchors[i * 2 + 1], Vector3.up, Vector3.right, Vector3.forward, 1.0f, Handles.DotHandleCap, Handles.SnapValue(1.0f, 1.0f));
            if (EditorGUI.EndChangeCheck())
            {
                if (i < path.points.Count - 2)
                {
                    Vector3 endDist = path.anchors[i * 2 + 2] - path.points[i + 1];
                    Vector3 startDist = path.anchors[i * 2 + 1] - path.points[i + 1];
                    Vector3 offset = (endDist - startDist).normalized;
                    path.anchors[i * 2 + 2] = path.points[i + 1] - (startDist - offset);
                }
            }

            Handles.DrawLine(path.points[i], path.anchors[i * 2]);
            Handles.DrawLine(nextPoint, path.anchors[i * 2 + 1]);
            Handles.DrawBezier(path.points[i], nextPoint, path.anchors[i * 2], path.anchors[i * 2 + 1], Color.green, null, 1.0f);
        }
    }
}

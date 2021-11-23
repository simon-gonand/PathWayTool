using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    public List<Vector3> points;
    public List<Vector3> anchors;

    public float strenght;
    public float speed;

    private float moveAmount;

    private void Start()
    {
        transform.position = points[0];
    }

    private Vector3 DrawBeziers(Vector3 a, Vector3 b, Vector3 startAnchor, Vector3 endAnchor, float t)
    {
        Vector3 A = Vector3.Lerp(a, startAnchor, t);
        Vector3 B = Vector3.Lerp(startAnchor, endAnchor, t);
        Vector3 C = Vector3.Lerp(endAnchor, b, t);

        Vector3 AB = Vector3.Lerp(A, B, t);
        Vector3 BC = Vector3.Lerp(B, C, t);

        transform.forward = BC - AB;

        return Vector3.Lerp(AB, BC, t);
    }

    private void Update()
    {
        moveAmount = (moveAmount + (Time.deltaTime * speed)) % 1.0f;
        float fullMoveAmount = moveAmount * points.Count - 1;
        int indexPoint = Mathf.FloorToInt(fullMoveAmount);
        if (indexPoint < 0) indexPoint = 0;
        float moveAmountPoint = fullMoveAmount - indexPoint;
        if (points.Count < 2) return;
        if (indexPoint == points.Count - 1) indexPoint = 0;
        Vector3 nextPoint;
        if (indexPoint == 0 && indexPoint < points.Count - 1)
        {
            nextPoint = points[indexPoint + 1];
        }
        else
        {
            nextPoint = points[indexPoint + 1];
        }

        transform.position = DrawBeziers(points[indexPoint], nextPoint, anchors[indexPoint * 2], anchors[indexPoint * 2 + 1], moveAmountPoint);
    }
}

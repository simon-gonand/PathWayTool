using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path path;
    public Transform self;

    private float moveAmount = 0.0f;
    private int linkIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (path.allPoints.Count > 0)
            self.position = path.allPoints[0];
    }

    private Vector3 CalculatePositionOnBeziers(Vector3 a, Vector3 b, Vector3 startAnchor, Vector3 endAnchor, float t)
    {
        Vector3 A = Vector3.Lerp(a, startAnchor, t);
        Vector3 B = Vector3.Lerp(startAnchor, endAnchor, t);
        Vector3 C = Vector3.Lerp(endAnchor, b, t);

        Vector3 AB = Vector3.Lerp(A, B, t);
        Vector3 BC = Vector3.Lerp(B, C, t);

        transform.forward = BC - AB;

        return Vector3.Lerp(AB, BC, t);
    }

    // Update is called once per frame
    void Update()
    {
        if (linkIndex == path.links.Count) return;
        moveAmount = (moveAmount + (Time.deltaTime * path.links[linkIndex].speed)) % 1.0f;
        
        float fullMoveAmount = moveAmount * path.allPoints.Count - 1;
        int indexPoint = Mathf.FloorToInt(fullMoveAmount);
        if (indexPoint < 0) indexPoint = 0;
        float moveAmountPoint = fullMoveAmount - indexPoint;
        if (path.allPoints.Count < 2) return;
        Vector3 nextPoint;
        if (indexPoint == 0 && indexPoint < path.allPoints.Count - 1)
        {
            nextPoint = path.allPoints[indexPoint + 1];
        }
        else
        {
            nextPoint = path.allPoints[indexPoint + 1];
        }

        self.position = CalculatePositionOnBeziers(path.allPoints[indexPoint], nextPoint, path.allAnchors[indexPoint * 2], path.allAnchors[indexPoint * 2 + 1], moveAmountPoint);
        if (moveAmount > 0.98f * (((float)linkIndex + 1) / ((float)path.links.Count)))
        { 
            ++linkIndex;
        }
    }
}

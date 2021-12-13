using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path path;
    public Transform self;
    public Transform camPosition;

    private float moveAmount = 0.0f;
    private int linkIndex = 0;

    private Vector3 initialOffset;
    private Vector3 initialPos;
    private float initialPosY;

    private float tParam = 0.0f;
    private bool coroutineAllowed = true;
    int allPointIndex = 0;

    private bool pathEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        if (path.allPoints.Count > 0)
            self.position = path.allPoints[0];

        initialPosY = self.position.y;

        // Get the offset between the camera and the boat
        initialOffset = camPosition.position - self.position;
        initialPos = camPosition.position;
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

    private void CameraFollow(float moveAmount)
    {
        // Update the position of the camera according to the boat on Z
        camPosition.position = new Vector3(self.position.x + initialOffset.x, initialPos.y,
            self.position.z + initialOffset.z);
        Vector3 camOffset = camPosition.position;
        camOffset.x += path.links[linkIndex].XCameraOffset.Evaluate(moveAmount);
        camOffset.z += path.links[linkIndex].YCameraOffset.Evaluate(moveAmount);
        camPosition.position = camOffset;
    }

    private IEnumerator FollowCurve()
    {
        coroutineAllowed = false;

        while(tParam < 1)
        {
            tParam += Time.deltaTime * path.links[linkIndex].speed;
            Vector3 posOnCurve = Mathf.Pow(1 - tParam, 3) * path.allPoints[allPointIndex] +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * path.allAnchors[allPointIndex * 2] +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * path.allAnchors[allPointIndex * 2 + 1] +
                Mathf.Pow(tParam, 3) * path.allPoints[allPointIndex + 1];
            posOnCurve.y = initialPosY;
            self.position = posOnCurve;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0.0f;
        ++allPointIndex;
        Debug.Log(linkIndex);
        Debug.Log(path.links.Count);
        if (allPointIndex == path.allPoints.Count - 1)
        {
            if (path.loop)
            {
                allPointIndex = 0;
                linkIndex = 0;
            }
            else
                pathEnd = true;
        }
        else if (linkIndex < path.links.Count - 1 && path.allPoints[allPointIndex] == path.links[linkIndex + 1].start.self.position)
            ++linkIndex;

        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (pathEnd) return;
        if (coroutineAllowed)
            StartCoroutine(FollowCurve());
        
        /*if (linkIndex >= path.links.Count)
        {
            if (path.loop)
            {
                linkIndex = 0;
            }
            else
                return;
        }
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
        CameraFollow(moveAmount);
        if (moveAmount > 0.98f * (((float)linkIndex + 1) / ((float)path.links.Count)))
        { 
            ++linkIndex;
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path path;
    public Transform self;
    public Transform camPosition;

    private int linkIndex = 0;
    private int linkCurveIndex = 0;

    private Vector3 initialOffset;
    private Vector3 initialPos;
    private float initialPosY;

    private float tParam = 0.0f;
    private float currentSpeed = 0.0f;
    private bool coroutineAllowed = true;
    int allPointIndex = 0;

    private bool pathEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        if (path.bakePath.Count > 0)
        {
            self.position = path.bakePath[0];
            currentSpeed = path.links[0].speed;
        }

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

    /*private void SetSpeed()
    {
        currentSpeed = path.links[linkIndex].curveLenghts[0] / path.links[linkIndex].curveLenghts[linkCurveIndex] * path.links[linkIndex].speed * 100;
    }*/

    private void SetPathPosition()
    {
        tParam = path.links[linkIndex].speed * Time.deltaTime;
        Vector3 nextPoint = path.bakePath[allPointIndex];
        nextPoint.y = initialPosY;
        if (!Mathf.Approximately(self.position.x, nextPoint.x) && !Mathf.Approximately(self.position.z, nextPoint.z))
        {
            self.position = Vector3.MoveTowards(self.position, nextPoint, tParam);
        }
        else
        {
            int nextLinkIndex = linkIndex + 1;
            if (linkIndex == path.links.Count - 1)
            {
                if (path.loop)
                    nextLinkIndex = 0;
                else
                    nextLinkIndex = -1;
            }
            ++allPointIndex;
            if (allPointIndex == path.bakePath.Count)
            {
                if (path.loop)
                {
                    allPointIndex = 0;
                    linkIndex = 0;
                    linkCurveIndex = 0;
                }
                else
                {
                    pathEnd = true;
                }
                return;
            }

            if (nextLinkIndex >= 0 && path.bakePath[allPointIndex].x == path.links[nextLinkIndex].start.self.position.x &&
                path.bakePath[allPointIndex].z == path.links[nextLinkIndex].start.self.position.z)
            {
                ++linkIndex;
                linkCurveIndex = 0;
                currentSpeed = path.links[linkIndex].speed;
            }
            else if (path.bakePath[allPointIndex].x == path.links[linkIndex].pathPoints[linkCurveIndex + 1].x &&
                     path.bakePath[allPointIndex].z == path.links[linkIndex].pathPoints[linkCurveIndex + 1].z)
            { 
                ++linkCurveIndex;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pathEnd) return;
        SetPathPosition();
        
        /*if (coroutineAllowed)
            StartCoroutine(FollowCurve());*/
        
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

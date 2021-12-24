using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path path;
    public Transform self;
    public Transform camPosition;

    private float tParam = 0.0f;
    private int linkIndex = 0;
    private int allPointIndex = 0;
    private int linkCurveIndex = 0;

    private Vector3 initialOffset;
    private Vector3 initialPos;
    private float initialPosY;


    private bool pathEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        if (path.bakePath.Count > 0)
        {
            self.position = path.bakePath[0];
        }

        initialPosY = self.position.y;

        // Get the offset between the camera and the boat
        initialOffset = camPosition.position - self.position;
        initialPos = camPosition.position;
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

    private float GetSpeedFactor()
    {
        return (10.0f / path.links[linkIndex].curveLengths[linkCurveIndex]);
    }

    private bool CheckNextPoint()
    {
        if (allPointIndex == path.bakePath.Count - 2)
        {
            if (path.loop)
            {
                allPointIndex = 0;
                linkCurveIndex = 0;
                linkIndex = 0;
            }
            else
            {
                pathEnd = true;
            }
        }
        else
        {
            ++allPointIndex;
            if (linkCurveIndex == path.links[linkIndex].curveLengths.Count - 1)
            {
                if (linkIndex == path.links.Count - 1) return pathEnd;
                else if (path.bakePath[allPointIndex].x == path.links[linkIndex + 1].pathPoints[0].x &&
                path.bakePath[allPointIndex].z == path.links[linkIndex + 1].pathPoints[0].z)
                {
                    linkCurveIndex = 0;
                    ++linkIndex;
                }
            }
            else
            {
                if (path.bakePath[allPointIndex].x == path.links[linkIndex].pathPoints[linkCurveIndex + 1].x &&
                path.bakePath[allPointIndex].z == path.links[linkIndex].pathPoints[linkCurveIndex + 1].z)
                {
                    ++linkCurveIndex;
                }
            }
        }
        return pathEnd;
    }

    private void SetPathPosition()
    {
        tParam += Time.deltaTime * path.links[linkIndex].speed * GetSpeedFactor();
        
        if (tParam >= 1.0f)
        {
            tParam = 0.0f;
            if (!CheckNextPoint()) return;
        }

        self.position = Vector3.Lerp(path.bakePath[allPointIndex], path.bakePath[allPointIndex + 1], tParam);
    }

    // Update is called once per frame
    void Update()
    {
        if (pathEnd) return;
        SetPathPosition();
    }
}

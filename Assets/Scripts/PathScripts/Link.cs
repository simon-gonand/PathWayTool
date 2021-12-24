using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public Waypoint start;
    public Waypoint end;

    public List<Vector3> pathPoints = new List<Vector3>();
    public List<Vector3> anchors = new List<Vector3>();

    public float speed;
    public AnimationCurve XCameraOffset;
    public AnimationCurve YCameraOffset;

    public List<float> curveLengths = new List<float>();

    public bool Equals(Waypoint start, Waypoint end)
    {
        return this.start == start && this.end == end;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

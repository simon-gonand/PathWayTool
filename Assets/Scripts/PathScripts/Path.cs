using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    public List<Link> links = new List<Link>();

    public List<Vector3> allPoints = new List<Vector3>();
    public List<Vector3> allAnchors = new List<Vector3>();

    public bool loop;
    public Link loopLink;

    public List<Vector3> bakePath = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void FillAllPointsList()
    {
        allPoints.Clear();
        allAnchors.Clear();
        for (int i = 0; i < links.Count; ++i)
        {
            if (i == 0)
            {
                allPoints.Add(links[i].pathPoints[0]);
            }
            for (int j = 1; j < links[i].pathPoints.Count; ++j)
            {
                if (i == links.Count - 1 && j == links[i].pathPoints.Count - 1) continue;
                allPoints.Add(links[i].pathPoints[j]);
            }
            for (int j = 0; j < links[i].anchors.Count; ++j)
            {
                allAnchors.Add(links[i].anchors[j]);
            }
        }
        allPoints.Add(waypoints[waypoints.Count - 1].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

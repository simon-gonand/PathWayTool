using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Link : MonoBehaviour
{
    public Waypoint start;
    public Waypoint end;

    public List<Vector3> pathPoints = new List<Vector3>();

    public Path parentPath;

    public bool Equals(Waypoint start, Waypoint end)
    {
        return this.start == start && this.end == end;
    }


    private void OnDestroy()
    {
        parentPath.links.Remove(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 1; i < pathPoints.Count; ++i)
        {
            Gizmos.DrawLine(pathPoints[i - 1], pathPoints[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

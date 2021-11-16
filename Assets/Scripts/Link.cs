using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Link : MonoBehaviour
{
    public Waypoint start;
    public Waypoint end;

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
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

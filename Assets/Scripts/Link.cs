using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public Waypoint start;
    public Waypoint end;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

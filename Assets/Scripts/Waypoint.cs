using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Path parentPath;
    public Transform self;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }

    // Start is called before the first frame update
    void Start()
    {
        self = transform;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}

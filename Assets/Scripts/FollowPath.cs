using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path m_Path;
    public Transform self;
    public float timer = 1.0f;

    private int indexWaypoint = 0;
    private int indexLinkPoint = 1;

    // Start is called before the first frame update
    void Start()
    {
        self.position = m_Path.links[0].start.self.position;
    }

    private void SetPathPosition()
    {
        if (indexLinkPoint < m_Path.links[indexWaypoint].pathPoints.Count)
        {
            if (self.position != m_Path.links[indexWaypoint].pathPoints[indexLinkPoint])
                self.position = Vector3.Lerp(self.position, m_Path.links[indexWaypoint].pathPoints[indexLinkPoint], timer * Time.deltaTime);
            else
                ++indexLinkPoint;
        }
        else
        {
            ++indexWaypoint;
            indexLinkPoint = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (indexWaypoint < m_Path.links.Count)
        {
            SetPathPosition();
        }
    }
}

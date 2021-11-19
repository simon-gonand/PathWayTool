using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Path m_Path;
    public Transform self;
    public float speed = 1.0f;

    private float timer = 0.0f;
    private int indexWaypoint = 0;
    private int indexLinkPoint = 1;
    private Vector3 previousPointPosition;

    // Start is called before the first frame update
    void Start()
    {
        self.position = m_Path.links[0].start.self.position;
        previousPointPosition = self.position;
    }

    private void SetPathPosition()
    {
        timer += speed * Time.deltaTime;
        if (indexLinkPoint < m_Path.links[indexWaypoint].pathPoints.Count)
        {
            if (self.position != m_Path.links[indexWaypoint].pathPoints[indexLinkPoint])
                self.position = Vector3.MoveTowards(previousPointPosition, m_Path.links[indexWaypoint].pathPoints[indexLinkPoint], 
                    timer);
            else
            {
                timer = 0;
                previousPointPosition = m_Path.links[indexWaypoint].pathPoints[indexLinkPoint];
                ++indexLinkPoint;
            }
        }
        else
        {
            timer = 0;
            ++indexWaypoint;
            if (indexWaypoint >= m_Path.links.Count) return;
            previousPointPosition = m_Path.links[indexWaypoint].pathPoints[0];
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

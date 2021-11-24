using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [SerializeField]
    private Transform self;
    [SerializeField]
    private Transform camPosition;
    [SerializeField]
    private Path path;

    private Vector3 initialOffset;
    private Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Get the offset between the camera and the boat
        initialOffset = camPosition.position - self.position;
        initialPos = camPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the position of the camera according to the boat on Z
        camPosition.position = new Vector3(self.position.x + initialOffset.x, initialPos.y,
            self.position.z + initialOffset.z);
    }
}

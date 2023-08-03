using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cam;
    public Transform getAxisCustom;
    public CinemachineVirtualCamera Cam;
    public Transform objectToFollow;
    public MidPoint midPoint;
    public float zoomOffSet;
    private Vector3 Dist;
    // Start is called before the first frame update
    void Start()
    {
        //CinemachineCore.GetInputAxis = getAxisCustom;
    }
    // Update is called once per frame
    void Update()
    {
        /*
        if (midPoint.Dist > 4f && midPoint.dist < 15)
        {
            Cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = zoomOffSet * midPoint.dist);
        }
        */
    }
}

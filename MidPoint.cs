using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPoint : MonoBehaviour
{
    private Vector3 midPoint;
    public List<Transform> targetObjects;
    public Transform targetObject;
    public Transform rotatingObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToTarget = targetObjects[0].position - rotatingObject.position;
    }
}

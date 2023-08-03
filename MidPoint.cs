using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPoint : MonoBehaviour
{
    private Vector3 midPoint;
    public List<Transform> targetObjects;
    public float dist;
    public float distSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionToTarget = targetObjects[0].position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up) * Quaternion.Euler(0f, 90f, 0f);
        transform.rotation = targetRotation;
        midPoint = (targetObjects[0].position + targetObjects[1].position) / 2f;
        transform.position = midPoint;
        dist = Vector3.Distance(targetObjects[0].position, targetObjects[1].position);
        distSpeed = (dist / Time.deltaTime);
    }
}

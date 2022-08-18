using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [SerializeField] private Vector3 restingPosition;
    [SerializeField] private Vector3 worldPosition;
    [SerializeField] private Transform desiredPoint;
    [SerializeField] private float strideLength;
    [SerializeField] private LayerMask solidLayer;
    [SerializeField] private Vector3 desired;
    // Start is called before the first frame update
    void Start()
    {
        worldPosition = transform.TransformPoint(restingPosition);
    }

    // Update is called once per frame
    void Update()
    {
        desired = transform.TransformPoint(desiredPoint.position);
        float dist = Vector3.Distance(ikTarget.position, desiredPoint.position);
        if(dist >= strideLength)
        {
            Step();
        }
        UpdateIKTarget();
        UpdateDesiredPoint();
    }
    
    private void UpdateIKTarget()
    {
        ikTarget.position = worldPosition;//Vector3.Lerp(ikTarget.position, restingPosition, Time.deltaTime);
    }

    private void UpdateDesiredPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(ikPole.position, Vector3.down, out hit, 5f, solidLayer))
        {
            desiredPoint.position = hit.point;
            Debug.DrawLine(desiredPoint.position, hit.point, Color.cyan);
        }
    }

    private void Step()
    {
        worldPosition = desiredPoint.position;


    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(ikTarget.position, desiredPoint.position);



    }
}

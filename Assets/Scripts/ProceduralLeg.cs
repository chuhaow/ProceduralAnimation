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
    [SerializeField] private float stepPercentage;
    private bool isStepping = false;

    [Header("Step Modifier")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepDur;
    [SerializeField] private float stepOffset;
    [SerializeField] private AnimationCurve stepHeightCurve;
    [SerializeField] private float lastStep = 0;
    [SerializeField] private float stepCoolDown = 0.01f;
    public float percent
    {
        get
        {
            return Mathf.Clamp01((Time.time - lastStep) / stepDur);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        worldPosition = transform.TransformPoint(restingPosition);
    }

    // Update is called once per frame
    void Update()
    {
        desired = desiredPoint.position;
        float dist = Vector3.Distance(ikTarget.position, desiredPoint.position);
        if(dist >= strideLength || isStepping)
        {
            isStepping = true;
            Step();
            Debug.Log("We are Stepping");
        }
        else if(!isStepping)
        {
            UpdateIKTarget();
            lastStep = Time.time;
        }
        
        UpdateDesiredPoint();
    }
    
    private void UpdateIKTarget()
    {
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldPosition, 1f);
    }

    private void UpdateDesiredPoint()
    {
        RaycastHit hit;
        Vector3 start = new Vector3(desiredPoint.position.x, ikPole.position.y, desiredPoint.position.z);
        float maxDist = Mathf.Abs(ikPole.position.y - desiredPoint.position.y)+1;
        if (Physics.Raycast(start, Vector3.down, out hit, maxDist, solidLayer))
        {
            desiredPoint.position = hit.point;
            Debug.DrawRay(start, Vector3.down, Color.red);
            
        }
    }

    private void Step()
    {
        worldPosition = desiredPoint.position;
        stepPercentage += Time.deltaTime;
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldPosition, percent) + Vector3.up * stepHeightCurve.Evaluate(percent) * stepHeight;
        if (Vector3.Distance(ikTarget.position, worldPosition) < 0.09f)
        {
            isStepping = false;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(ikTarget.position, desiredPoint.position);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    [Header("Leg positioning")]
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [Tooltip("The resting position of legs in Leg space")]
    [SerializeField] private Vector3 restingPosition;
    private Vector3 worldPosition;
    [SerializeField] private Transform desiredPoint;
    [Tooltip("The Layer that can be stepped on")]
    [SerializeField] private LayerMask solidLayer;

    private float dist;
    private bool isStepping = false;

    [Header("Step Modifier")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepDur;
    [SerializeField] private float stepCoolDown;
    [SerializeField] private float strideLength;
    [Tooltip("The Curve which the step follows")]
    [SerializeField] private AnimationCurve stepHeightCurve;
    [SerializeField] private float lastStep = 0;
    public float Percent
    {
        get
        {
            return Mathf.Clamp01((Time.time - lastStep) / stepDur);
        }
    }

    public bool CanStep
    {
        get
        {
            return dist >= strideLength;
        }
    }

    public bool IsInStep
    {
        get
        {
            return isStepping;
        }
    }

    public Vector3 FootPosition
    {
        get
        {
            return ikTarget.position;
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
        
        dist = Vector3.Distance(ikTarget.position, desiredPoint.position);
        if(isStepping && Time.time - lastStep >= stepCoolDown)
        {
            
            Step();
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
        worldPosition.y = desiredPoint.position.y;
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

    public void Step()
    {
        isStepping = true;
        worldPosition = desiredPoint.position;
        
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldPosition, Percent) + Vector3.up * stepHeightCurve.Evaluate(Percent) * stepHeight;
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

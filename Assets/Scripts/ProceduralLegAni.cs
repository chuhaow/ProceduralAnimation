using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegAni : MonoBehaviour
{
    [SerializeField] private LayerMask solidLayer;
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [SerializeField] private Vector3 worldTarget = Vector3.zero;
    [SerializeField] public Vector3 restingPosition = Vector3.forward;
    [SerializeField] private Vector3 worldVelocity = Vector3.right;

    [SerializeField] private Vector3 offset;

    [Header("Step Modifier")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepDur;
    [SerializeField] private float stepOffset;
    [SerializeField] private AnimationCurve stepHeightCurve;
    [SerializeField] private float lastStep = 0;
    [SerializeField] private float stepCoolDown = 0.01f;

    public Vector3 worldRestingPos //transform to world space
    {
        get
        {
            return transform.TransformPoint(restingPosition) + offset;
        }
    }

    public Vector3 destinationPosition
    {
        get
        {
            return worldRestingPos + worldVelocity ;
        }
    }

    public float StepDur
    {
        set
        {
            stepDur = value;
        }
    }

    public Vector3 WorldVelocity
    {
        set
        {
            worldVelocity = value;
        }
    }

    public float LastStep
    {
        set
        {
            lastStep = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        worldVelocity = Vector3.zero;
        lastStep = Time.time + stepCoolDown * stepOffset;
        Step();
    }


    // Update is called once per frame
    void Update()
    {

        UpdateIKTarget();
        if(Time.time > lastStep + stepCoolDown)
        {
            Step();
        }
    }

    private void UpdateIKTarget()
    {
        float percent = Mathf.Clamp01((Time.time - lastStep) / stepDur);
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldTarget, percent) + Vector3.up * stepHeightCurve.Evaluate(percent) * stepHeight;
    }

    public void Step()
    {
        Vector3 dir = destinationPosition - ikPole.position;
        RaycastHit hit;
        if(Physics.Raycast (ikPole.position,dir,out hit, dir.magnitude, solidLayer))
        {
            
            worldTarget = hit.point;
        }
        else
        {
           

            worldTarget = worldRestingPos;
        }
        lastStep = Time.time;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(worldRestingPos, destinationPosition);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(worldRestingPos, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destinationPosition, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(ikPole.position, destinationPosition);
    }
}

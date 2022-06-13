using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegAni : MonoBehaviour
{
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [SerializeField] private Vector3 worldTarget = Vector3.zero;
    [SerializeField] public Vector3 restingPosition = Vector3.forward;
    [SerializeField] private Vector3 worldVelocity = Vector3.right;
    [SerializeField] private Vector3 temp;

    [Header("Step Modifier")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepDur;
    [SerializeField] private AnimationCurve stepHeightCurve;
    [SerializeField] private float lastStep = 0;
    [SerializeField] private float stepCoolDown = 0.1f;

    public Vector3 worldRestingPos //transform to world space
    {
        get
        {
            return transform.TransformPoint(restingPosition) - Vector3.up;
        }
    }

    public Vector3 destinationPosition
    {
        get
        {
            return restingPosition + worldVelocity ;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        worldVelocity = Vector3.zero;
        Step();
    }


    // Update is called once per frame
    void Update()
    {
        temp = destinationPosition;
        UpdateIKTarget();
        if(Time.time > lastStep + stepCoolDown)
        {
            Step();
        }
    }

    private void UpdateIKTarget()
    {
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldTarget, Time.deltaTime) + Vector3.up * stepHeightCurve.Evaluate(Time.deltaTime) * stepHeight;
    }

    private void Step()
    {
        Vector3 dir = destinationPosition - ikPole.position;
        RaycastHit hit;
        if(Physics.Raycast (ikPole.position,dir,out hit, dir.magnitude * 2f))
        {
            worldTarget = hit.point;
        }
        else
        {
            worldTarget = worldRestingPos;
            Debug.LogError("HERE");
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

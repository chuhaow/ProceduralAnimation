using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegAni : MonoBehaviour
{
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [SerializeField] private Vector3 worldTarget = Vector3.zero;
    [SerializeField] private Vector3 restingPostion = Vector3.forward;
    [SerializeField] private Vector3 worldVelocity = Vector3.right;

    [Header("Step Modifier")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepDur;
    [SerializeField] private AnimationCurve stepHeightCurve;

    public Vector3 restingPosition //transform to world space
    {
        get
        {
            return transform.TransformPoint(restingPosition);
        }
    }

    public Vector3 destinationPosition
    {
        get
        {
            return restingPosition + worldVelocity;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        worldVelocity = Vector3.zero;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void updateIKTarget()
    {
        ikTarget.position = Vector3.Lerp(ikTarget.position, worldTarget, Time.deltaTime) + Vector3.up * stepHeightCurve.Evaluate(Time.deltaTime) * stepHeight;
    }
}

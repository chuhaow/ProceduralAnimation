using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFabric : MonoBehaviour
{
    
    [SerializeField] private int chainLength;
    [SerializeField] private Transform target;
    [SerializeField] private Transform pole;

    [SerializeField] private int iterations;
    [SerializeField] private float delta;

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;
    private void Awake()
    {
        Init();
    }
    void Init()
    {
        bones = new Transform[chainLength + 1]; // + 1 because bones are the joints and chainlength is connection between
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];
        startRotationTarget = target.rotation;
        completeLength = 0;

        Transform current = transform;
        for(int i = bones.Length -1; i >= 0; i--)
        {
            startRotationBone[i] = current.rotation;
            bones[i] = current;
            if(i == bones.Length - 1) 
            {
                startDirSucc[i] = target.position - current.position;
            }
            else
            {
                startDirSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = ((bones[i + 1]).position - current.position).magnitude;
                completeLength += bonesLength[i];
            }
            current = current.parent;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        bool isLastCloseEnough = false;
        if(target == null)
        {
            return;
        }
        if(bonesLength.Length != chainLength)
        {
            Init();
        }
        //get position
        for(int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;
        }
        var rootRotation = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var rootRotationDiff = rootRotation * Quaternion.Inverse(startRotationRoot);

        // check if dist to target from base bone is greater than limb length
        if((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            Vector3 dir = (target.position - bones[0].position).normalized;

            //move parent bone to new position 
            for(int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + dir * bonesLength[i-1];
            }
        }
        else
        {
            for(int iter = 0; iter < iterations && !isLastCloseEnough; iter++)
            {
                //adjust backward
                for(int i = positions.Length-1; i >0; i--)
                {
                    if(i == positions.Length - 1)
                    {
                        positions[i] = target.position;
                    }
                    else
                    {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                    }
                }

                //adjust forward
                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];
                }

                // if last position is close than delta stop
                isLastCloseEnough = (positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta;

            }
        }
        //move towards pole
        if (pole != null)
        {
            for (int i = 1; i < positions.Length - 1; i++)
            {
                
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }
    

        //set position
        for(int i = 0; i < positions.Length; i++)
        {
            if( i == positions.Length - 1)
            {
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            }
            else
            {
                bones[i].rotation = Quaternion.FromToRotation(startDirSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
                bones[i].position = positions[i];
            }
            
        }

    }


}

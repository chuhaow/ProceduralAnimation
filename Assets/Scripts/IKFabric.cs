using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFabric : MonoBehaviour
{
    
    [SerializeField] private int chainLength;
    [SerializeField] private Transform target;

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    private void Awake()
    {
        Init();
    }
    void Init()
    {
        bones = new Transform[chainLength + 1]; // + 1 because bones are the joints and chainlength is connection between
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];

        Transform current = transform;
        for(int i = bones.Length -1; i >= 0; i--)
        {
            bones[i] = current;
            if(i == bones.Length - 1) // i
            {

            }
            else
            {
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

        //set position
        for(int i = 0; i < positions.Length; i++)
        {
            bones[i].position = positions[i];
        }

    }
}

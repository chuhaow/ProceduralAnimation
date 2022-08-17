using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    [SerializeField] private Transform ikTarget;
    [SerializeField] private Transform ikPole;
    [SerializeField] private Vector3 restingPosition;
    [SerializeField] private Vector3 worldPosition;
    // Start is called before the first frame update
    void Start()
    {
        worldPosition = transform.TransformPoint(restingPosition);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIKTarget();
        
    }
    
    private void UpdateIKTarget()
    {
        ikTarget.position = worldPosition;//Vector3.Lerp(ikTarget.position, restingPosition, Time.deltaTime);
        
    }

    private void Step()
    {

    }
}

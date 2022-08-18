using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    private float lastStep;
    [Header("Legs")]
    [SerializeField] private ProceduralLeg[] legs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveLegs();
 
    }

    private void MoveLegs()
    {
        for(int legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            if(legs[legIndex] != null)
            {
                if (legs[legIndex].CanStep)
                {
                    legs[legIndex].Step();
                    lastStep = Time.time;
                    
                }
            }
        }
    }


}

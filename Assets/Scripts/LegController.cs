using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    private float lastStep;

    [SerializeField] private float startHeight;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float currtHeight;
    [Header("Legs")]
    [SerializeField] private ProceduralLeg[] legs;
    // Start is called before the first frame update
    void Start()
    {
        startHeight = Mathf.Abs(transform.position.y - AvgFootYPosition());
        Debug.Log(AvgFootYPosition());

    }

    // Update is called once per frame
    void Update()
    {
        MoveLegs();
        positionBody();

    }

    private void positionBody()
    {
        float currHeight = Mathf.Abs(transform.position.y - AvgFootYPosition());
        currtHeight = currHeight;
        if(currHeight < minHeight)
        {
            Vector3 desiredBodyPosition = new Vector3(transform.position.x, transform.position.y + Mathf.Abs(startHeight - currHeight), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredBodyPosition, Time.deltaTime*2);
        }else if ( currHeight > maxHeight)
        {
            Vector3 desiredBodyPosition = new Vector3(transform.position.x, transform.position.y - Mathf.Abs(startHeight - currHeight), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredBodyPosition, Time.deltaTime*2);
        }

    }

    private void MoveLegs()
    {
        for(int legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            if(legs[legIndex] != null)
            {
                int nextLeg = (legIndex + 1) % legs.Length;
                
                if (legs[legIndex].CanStep && !legs[nextLeg].isInStep)
                {
                    legs[legIndex].Step();
                    lastStep = Time.time;
                    
                }
            }
        }
    }

    private float AvgFootYPosition()
    {
        float avgHeight =0;
        for (int legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            if (legs[legIndex] != null)
            {
                avgHeight += legs[legIndex].footPosition.y;
            }
        }
        avgHeight /= legs.Length;
        return avgHeight;
    }

    private void CalulateAllTriangleNormal()
    {

    }

    private float CalulateLeftRightLegDiff()
    {
        Vector3 rightAvg = new Vector3();
        Vector3 leftAvg = new Vector3(); 
        for (int legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            if (legs[legIndex] != null)
            {
                if(legs[legIndex].footPosition.x > transform.position.x) // right leg
                {
                    rightAvg += legs[legIndex].footPosition;
                }
                else // left leg
                {
                    leftAvg += legs[legIndex].footPosition;
                }
            }
        }
        rightAvg /= legs.Length;
        leftAvg /= legs.Length;
        
        return Vector3.Angle(rightAvg, leftAvg);
    }

}

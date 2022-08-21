using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    private float lastStep;
    private List<int[]> triangles = new List<int[]>();
    [SerializeField] private float startHeight;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float currtHeight;
    [SerializeField] private Vector3 norm1;
    [SerializeField] private Vector3 fake;
    [SerializeField] private Transform body;
    [SerializeField] private float offset;
    [Header("Legs")]
    [SerializeField] private ProceduralLeg[] legs;
    // Start is called before the first frame update
    void Start()
    {
        startHeight = Mathf.Abs(transform.position.y - AvgFootYPosition());
        FindAllTriangles();
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
        if (isStationary())
        {
            Vector3 norm = CalulateAllTriangleNormal();
            norm1 = norm;
            Debug.DrawRay(body.position, norm * 2, Color.red);
            body.up = Vector3.Lerp(body.up,norm, Time.deltaTime * 2);
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

    private bool isStationary()
    {
        bool result = true; 
        foreach(ProceduralLeg leg in legs)
        {
            if (leg.isInStep)
            {
                result = false;
            }
        }
        return result;
    }

    private Vector3 CalulateAllTriangleNormal()
    {
        Vector3 norm = Vector3.zero;
        if(legs.Length == 2)
        {
            Vector3 falseLeg = new Vector3(legs[0].footPosition.x + offset, Mathf.Abs((legs[1].footPosition.y - legs[0].footPosition.y)/2), legs[0].footPosition.z);
            fake = falseLeg;
            norm = Vector3.Cross(legs[1].footPosition - legs[0].footPosition,falseLeg- legs[0].footPosition).normalized;
            
        }
        else
        {
            foreach (int[] tri in triangles)
            {

                if (tri.Length == 3)
                {
                    norm += (Vector3.Cross(legs[tri[1]].footPosition - legs[tri[0]].footPosition, legs[tri[2]].footPosition - legs[tri[0]].footPosition)).normalized;
                }

            }
            norm /= triangles.Count;
        }
        return norm;
    }

    private float det3D(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        
        float det = p0.x * ((p1.y * p2.z) - (p2.y * p1.z)) -
              p1.x * ((p0.y * p2.z) - (p2.y * p0.z)) +
              p2.x * ((p0.y * p1.z) - (p1.y * p0.z));
        return det;
    }

    //Find all possible triangles formed by foot position
    private void FindAllTriangles()
    {
        for(int i = 0; i < legs.Length; i++)
        {
            for(int j = i+1; j < legs.Length; j++)
            {
                for(int k = j + 1; k < legs.Length; k++)
                {
                    if(det3D(legs[i].footPosition, legs[j].footPosition, legs[k].footPosition) != 0)
                    {
                        triangles.Add(new int[]{ i, j, k });
                    }
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(body.position, body.up);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fake, 1f);
    }

}

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
    [SerializeField] private Vector3 norm;
    [SerializeField] private Transform body;
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
        CalulateAllTriangleNormal();
        body.up = norm;
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
        norm = Vector3.zero;
        if(legs.Length <= 2)
        {
            return;
        }
        foreach (int[] tri in triangles)
        {

            if (tri.Length == 3)
            {
                norm += (Vector3.Cross(legs[tri[1]].footPosition - legs[tri[0]].footPosition, legs[tri[2]].footPosition - legs[tri[0]].footPosition)).normalized;
            }

        }
        norm /= triangles.Count;
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    private List<int[]> triangles = new List<int[]>();
    private float startHeight;


    
    private Vector3 fake;
    [SerializeField] private Transform body;
    [Tooltip("The offset for the fake leg used for bipeds to determine tilt")]
    [SerializeField] private float offset;
    [Header("Legs")]
    [SerializeField] private ProceduralLeg[] legs;
    [Header("Creature Height")]

    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    // Start is called before the first frame update
    void Start()
    {
        startHeight = Mathf.Abs(transform.position.y - AvgFootYPosition());
        FindAllTriangles();

    }

    // Update is called once per frame
    void Update()
    {
        MoveLegs();
        PositionBody();
    }

   
    private void PositionBody()
    {
        float currHeight = Mathf.Abs(transform.position.y - AvgFootYPosition());

        if(currHeight < minHeight)
        {
            Vector3 desiredBodyPosition = new Vector3(transform.position.x, transform.position.y + Mathf.Abs(startHeight - currHeight), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredBodyPosition, Time.deltaTime*2);
        }else if ( currHeight > maxHeight)
        {
            Vector3 desiredBodyPosition = new Vector3(transform.position.x, transform.position.y - Mathf.Abs(startHeight - currHeight), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredBodyPosition, Time.deltaTime*2);
        }
        if (IsStationary())
        {
            Vector3 norm = CalulateBodyUp();
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
                
                if (legs[legIndex].CanStep && !legs[nextLeg].IsInStep)
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
                avgHeight += legs[legIndex].FootPosition.y;
            }
        }
        avgHeight /= legs.Length;
        return avgHeight;
    }

    private bool IsStationary()
    {
        bool result = true; 
        foreach(ProceduralLeg leg in legs)
        {
            if (leg.IsInStep)
            {
                result = false;
            }
        }
        return result;
    }

    /// <summary>
    /// Find the Up Vector for the body
    /// </summary>
    private Vector3 CalulateBodyUp()
    {
        Vector3 norm = Vector3.zero;
        if(legs.Length == 2)
        {
            Vector3 falseLeg = new Vector3(legs[0].FootPosition.x + offset, Mathf.Abs((legs[1].FootPosition.y - legs[0].FootPosition.y)/2), legs[0].FootPosition.z);
            fake = falseLeg;
            norm = Vector3.Cross(legs[1].FootPosition - legs[0].FootPosition,falseLeg- legs[0].FootPosition).normalized;
            
        }
        else
        {
            foreach (int[] tri in triangles)
            {

                if (tri.Length == 3)
                {
                    norm += (Vector3.Cross(legs[tri[1]].FootPosition - legs[tri[0]].FootPosition, legs[tri[2]].FootPosition - legs[tri[0]].FootPosition)).normalized;
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

    /// <summary>
    /// Find all possible Triangles that can be formed by feet positions
    /// </summary>
    private void FindAllTriangles()
    {
        for(int i = 0; i < legs.Length; i++)
        {
            for(int j = i+1; j < legs.Length; j++)
            {
                for(int k = j + 1; k < legs.Length; k++)
                {
                    if(det3D(legs[i].FootPosition, legs[j].FootPosition, legs[k].FootPosition) != 0)
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
        if(legs.Length > 2)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(fake, 1f);
        }
        
    }

}

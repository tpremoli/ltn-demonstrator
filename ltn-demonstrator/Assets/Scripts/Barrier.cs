using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isPointInBarrier(Vector3 point)
    {
        Collider barrierCollider = GetComponent<Collider>();
        if(barrierCollider == null)
        {
            return false;
        }

        // Check if the point is within the barrier's collider
        return barrierCollider.bounds.Contains(point);
    }
    }

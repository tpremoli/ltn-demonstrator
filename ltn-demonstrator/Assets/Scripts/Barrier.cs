using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
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

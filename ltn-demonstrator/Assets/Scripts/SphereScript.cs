using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour, IDrag
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Implement OnDragStart method from IDrag interface
    public void onStartDrag()
    {
        rb.useGravity = false;
    }

    // Implement OnDragEnd method from IDrag interface
    public void onEndDrag()
    {
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
    }
}
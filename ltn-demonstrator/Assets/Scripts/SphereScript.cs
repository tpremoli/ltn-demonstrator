using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour, IDrag
{
    private Rigidbody rb;
    public GameObject spherePrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Implement OnDragStart method from IDrag interface
    public void onStartDrag()
    {
        if (gameObject == null)
        {
            Debug.Log("The GameObject has been destroyed");
        }
        else
        {
            Debug.Log("The GameObject has not been destroyed");
        }
        // If the GameObject has the "Draggable" tag, remove it
        if (gameObject.tag == "Draggable")
        {
            gameObject.tag = "";
        }
        rb.useGravity = false;
    }

    // Implement OnDragEnd method from IDrag interface
    public void onEndDrag()
    {
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 dragOrigin; // Declare dragOrigin here
    public Camera cam;

    private void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Origin: " + dragOrigin + " Mouse: " + cam.ScreenToWorldPoint(Input.mousePosition) + " Difference: " + difference);
            cam.transform.position += difference;
        }
    }
}
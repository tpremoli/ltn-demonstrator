using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;

    [SerializeField]
    private float mouseDragPhysicsSpeed = 10f;

    [SerializeField]
    private float mouseDragSpeed = 0.1f;
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;



    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.performed += MousePressed;
        mouseClick.Enable();
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        // Take the mouse position to the camera and convert it to a ray
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && (hit.collider.gameObject.tag == "Draggable" || 
                hit.collider.gameObject.layer == LayerMask.NameToLayer("Draggable") || 
                hit.collider.gameObject.GetComponent<IDrag>() != null))
            {
                StartCoroutine(DragUpdate(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        if (clickedObject == null)
        {
            yield break;
        }

        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        clickedObject.TryGetComponent<IDrag>(out var IDragComponent);
        IDragComponent?.onStartDrag();
        // while the mouse is pressed on the object
        while (mouseClick.ReadValue<float>() != 0 && clickedObject != null)
        {
            // Take the mouse position to the camera and convert it to a ray
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (rb != null)
            {
                // A to B is B - A
                // GetPoint returns a point at a distance from the origin of the ray
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                rb.velocity = direction * mouseDragPhysicsSpeed;
                yield return waitForFixedUpdate;
            }
            else
            {
                clickedObject.transform.position = Vector3.SmoothDamp(clickedObject.transform.position, ray.GetPoint(initialDistance),
                    ref velocity, 0.1f);
                yield return null;
            }
        }
        // If the clicked object is still not null
        

        if (clickedObject != null)
        {
            // Call the onEndDrag method of the IDrag component, if it exists
            IDragComponent?.onEndDrag();

            
            // Store the position of the object when the mouse is released
            Vector3 finalPosition = clickedObject.transform.position;
            Debug.Log("The object was dropped at: " + finalPosition);

            /*

            List<Edge> edges = Edge.allEdges; // Changed this line
            for (int i = 0; i < edges.Count; i++) { // Changed this line
                // get closest point on edge (where the barrier is located)
                if (clickedObject.isPointInBarrier(edges[i].GetClosestPoint(finalPosition))) {
                    edges[i].barricade = clickedObject;
                    edges[i].isBarricated = true;
                    edges[i].barricadeLocation = edges[i].convertToPositionAlongEdge(finalPosition);
                }
            }
            */
            
        }

        
        
    }
}

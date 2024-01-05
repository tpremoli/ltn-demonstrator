using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DragAndDROP : MonoBehaviour
{
    [SerializedField]
    private InputAction mouseClick;

    [SerializedField]
    private float mouseDragPhysicsSpeed = 10f;
    [SerializedField]
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
        mouseClick.Enable();
        mouseClick.performed += OnMouseClick;
    }

    private void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= OnMouseClick;
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        // Take the mouse position to the camera and convert it to a ray
        Ray ray = mainCamera.ScreenToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                StartCoroutine(DragUpdate(hit.collider.gameObject));
            }
    }

    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        // while the mouse is pressed on the object
        while (mouseClick.ReadValue<float>() != 0)
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
    }
    
}

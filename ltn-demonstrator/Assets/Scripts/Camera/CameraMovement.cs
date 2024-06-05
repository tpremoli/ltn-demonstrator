using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float minY;

    private float maxY = 175;

    [SerializeField]
    float sensitivity = 1;
    [SerializeField]
    float scrollSensitivity = 20;

    private Vector3 dragOrigin; // Declare dragOrigin here

    private float zoomLevel;

    public bool canMove = true; // Add this line

    public Camera cinematicCamera;

    private void Update()
    {
        if (!this.gameObject.activeSelf){
            this.gameObject.SetActive(true);
            cinematicCamera.gameObject.SetActive(false);
            MoveToCinematic();
        }

        if (canMove) // Add this line
        {
            PanCamera();
            SetCameraHeight();
            Zoom();
        }
    }

    const int maxZoomLevel = 1000;

    void SetCameraHeight()
    {
        var y = Mathf.Lerp(minY, maxY,1 - (zoomLevel / maxZoomLevel));
        cam.transform.position = new Vector3(cam.transform.position.x, y, cam.transform.position.z);
    }

    private Vector3 GetRayDirectionFromMouse()
    {
        var mosPos = Input.mousePosition;
        mosPos.z = 10;
        var moseWorldPoint = cam.ScreenToWorldPoint(mosPos);
        var raycastDir = (moseWorldPoint - cam.transform.position).normalized;

        return raycastDir;
    }
    private void MoveToCinematic(){
        if (cinematicCamera == null) return;
        cam.transform.position = cinematicCamera.transform.position;
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (Physics.Raycast(cam.transform.position, GetRayDirectionFromMouse(), out RaycastHit hit))
            {
                var hitPosition = hit.point;
                dragOrigin = hitPosition;

            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(cam.transform.position, GetRayDirectionFromMouse(), out RaycastHit hit))
            {
                var hitPosition = hit.point;
                Vector3 difference = dragOrigin - hitPosition;
                cam.transform.position += difference * sensitivity;
            }
        }
    }

    public void Zoom()
    {
        var scroll = Input.mouseScrollDelta.y * scrollSensitivity;
        zoomLevel += scroll;
        zoomLevel = Mathf.Clamp(zoomLevel, 0, maxZoomLevel);
    }
}
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTimeCinematic = 0.5f;
    [SerializeField] private float lingeringDuration = 15f;
    private Vector3 _currentVelocity = Vector3.zero;

    public Camera mainCamera;

    private float timePassed = 0;
    private Transform target;
    [SerializeField] private float verticalDistanceCinematic = 1000f; // Distance above the target

    private void Awake()
    {
        SwitchTarget();
    }

    private void MoveMainToCinematic(){
        // move transform to current positon
        mainCamera.gameObject.transform.position = this.gameObject.transform.position;
        
        // get the camera script
        CameraMovement camScript = mainCamera.GetComponent<CameraMovement>();
        
        
        camScript.SetZoomLevel(this.transform.position); // control zoom
        camScript.SleepPanningFor(10); // disable panning for a bit
    }

    private void LateUpdate()
    {
        // if the button's down, swap to main Camera
        if (Input.GetMouseButtonDown(0)){
            this.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            MoveMainToCinematic();
        }
        // check if the target has been destroyed
        if (target == null || target.gameObject == null)
        {
            SwitchTarget();
            return;
        }

        Vector3 desiredPosition = target.position + _offset + Vector3.up * verticalDistanceCinematic;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTimeCinematic);

        // get time passed in seconds
        timePassed += Time.unscaledDeltaTime;
        if (timePassed >= lingeringDuration) // Compare to a constant value in seconds
        {
            SwitchTarget();
            timePassed = 0;
        }
    }

    private void SwitchTarget()
    {
        Transform travellerManagerTransform = TravellerManager.Instance.GetManagerObject().transform;
        int nextIndex = Random.Range(0, travellerManagerTransform.childCount);
        SetTarget(travellerManagerTransform.GetChild(nextIndex).gameObject);
    }

    private void SetTarget(GameObject newTarget)
    {
        target = newTarget.transform;
        _offset = Vector3.zero;
        Debug.Log("Now following this target: " + target.name);
    }

}

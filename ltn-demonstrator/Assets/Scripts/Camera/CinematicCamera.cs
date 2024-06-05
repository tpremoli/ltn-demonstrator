using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTimeCinematic = 0.5f;
    private Vector3 _currentVelocity = Vector3.zero;

    private float timePassed = 0;
    private Transform target;
    [SerializeField] private float verticalDistanceCinematic = 1000f; // Distance above the target

    private void Awake()
    {
        SwitchTarget();
    }

    private void LateUpdate()
    {
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
        if (timePassed >= 4.0f) // Compare to a constant value in seconds
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

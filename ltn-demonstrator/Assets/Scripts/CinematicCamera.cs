using System.Collections.Generic;
using UnityEngine;
using System.IO; // For reading files

[System.Serializable]
public class CameraTarget_Cinematic
{
    public GameObject prefab;

    public CameraTarget_Cinematic(GameObject prefab)
    {
        this.prefab = prefab;
    }
}

[System.Serializable]
public class PositionData_Cinematic
{
    public List<PositionItem_Cinematic> positions;
}

[System.Serializable]
public class PositionItem_Cinematic
{
    public float x;
    public float y;
    public float z;
}

public class CinematicCamera : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTimeCinematic = 0.5f;
    private Vector3 _currentVelocity = Vector3.zero;

    private List<CameraTarget_Cinematic> inventory = new List<CameraTarget_Cinematic>();
    private int timeStepCounter = 0;
    private Transform target;
    [SerializeField] private float verticalDistanceCinematic = 1000f; // Distance above the target

    private void Awake()
    {
        SwitchTarget();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + _offset + Vector3.up * verticalDistanceCinematic;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTimeCinematic);

        timeStepCounter++;
        if (timeStepCounter >= 300)
        {
            SwitchTarget();
            timeStepCounter = 0;
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

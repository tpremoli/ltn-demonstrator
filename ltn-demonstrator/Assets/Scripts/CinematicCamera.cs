using System.Collections.Generic;
using UnityEngine;
using System.IO; // For reading files

[System.Serializable]
public class CameraTarget
{
    public GameObject prefab;

    public CameraTarget(GameObject prefab)
    {
        this.prefab = prefab;
    }
}

[System.Serializable]
public class PositionData
{
    public List<PositionItem> positions;
}

[System.Serializable]
public class PositionItem
{
    public float x;
    public float y;
    public float z;
}

public class CinematicCamera : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTime = 0.5f;
    private Vector3 _currentVelocity = Vector3.zero;

    private List<CameraTarget> inventory = new List<CameraTarget>();
    private int timeStepCounter = 0;
    private Transform target;
    [SerializeField] private float verticalDistance = 1000f; // Distance above the target

    private void Awake()
    {
        LoadPositions();
        if (inventory.Count > 0)
        {
            SetTarget(inventory[0].prefab.transform);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + _offset + Vector3.up * verticalDistance;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTime);

        timeStepCounter++;
        if (timeStepCounter >= 300)
        {
            SwitchTarget();
            timeStepCounter = 0;
        }
    }

    private void SwitchTarget()
    {
        GameObject travellerManagerObject = travellerManager.Instance.GetManagerObject();
        int nextIndex = Random.Range(0,travellerManagerObject.childCount)
        SetTarget(travellerManagerObject.transform.GetChild(nextIndex).gameObject);
    }

    private void SetTarget(GameObject newTarget)
    {
        target = newTarget.transform;
        _offset = Vector3.zero;
        Debug.Log("Now following this target: " + target.name);
    }

    private void LoadPositions()
    {
    }

}

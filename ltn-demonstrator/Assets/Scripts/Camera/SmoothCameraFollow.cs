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

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTime = 0.5f;
    private Vector3 _currentVelocity = Vector3.zero;

    [SerializeField] private List<CameraTarget> inventory = new List<CameraTarget>();
    private int timeStepCounter = 0;
    private Transform target;
    [SerializeField] private float verticalDistance = 1000f; // Distance above the target

    private void Awake()
    {
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
        if (inventory.Count <= 1) return;

        int currentIndex = inventory.FindIndex(item => item.prefab.transform == target);
        int nextIndex = (currentIndex + 1) % inventory.Count;
        SetTarget(inventory[nextIndex].prefab.transform);
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _offset = Vector3.zero;
        Debug.Log("Now following this target: " + target.name);
    }

    public void AddToInventory(GameObject prefab)
    {
        if (inventory.Find(item => item.prefab == prefab) == null)
        {
            inventory.Add(new CameraTarget(prefab));
        }
    }

    public void RemoveFromInventory(GameObject prefab)
    {
        inventory.RemoveAll(item => item.prefab == prefab);
    }
}

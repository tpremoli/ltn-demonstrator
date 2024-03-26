using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 _currentVelocity = Vector3.zero;

    [SerializeField] private List<GameObject> inventory = new List<GameObject>(); // This is your "inventory" of prefabs.
    private int timeStepCounter = 0;
    private Transform target;

    private void Awake()
    {
        // Set the initial target if the inventory is not empty
        if (inventory.Count > 0)
        {
            SetTarget(inventory[0].transform);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Move the camera smoothly towards the target
        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(current: transform.position, target: targetPosition, ref _currentVelocity, smoothTime);

        // Increment the time step counter
        timeStepCounter++;

        // Every 30 time steps, switch the target
        if (timeStepCounter >= 30)
        {
            SwitchTarget();
            timeStepCounter = 0; // Reset the counter
        }
    }

    private void SwitchTarget()
    {
        // If inventory is empty or has only one prefab, no need to switch
        if (inventory.Count <= 1) return;

        // Find the current target index
        int currentIndex = inventory.FindIndex(obj => obj.transform == target);

        // Calculate the next target index
        int nextIndex = (currentIndex + 1) % inventory.Count;

        // Set the next target
        SetTarget(inventory[nextIndex].transform);

        // Log the switch for debugging
        Debug.Log("Switched target to: " + target.name);
    }

    // Method to set the new target and update the offset
    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _offset = transform.position - target.position;
    }

    // Call this method to add a new prefab to the inventory
    public void AddToInventory(GameObject prefab)
    {
        if (!inventory.Contains(prefab))
        {
            inventory.Add(prefab);
        }
    }

    // Call this method to remove a prefab from the inventory
    public void RemoveFromInventory(GameObject prefab)
    {
        if (inventory.Contains(prefab))
        {
            inventory.Remove(prefab);
        }
    }
}

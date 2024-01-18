using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    public GameObject barrierPrefab;
    public List<GameObject> allBarriers; // Add this line

    public void Start()
    {
        allBarriers = new List<GameObject>(); // Initialize the list
        LoadBarriers();
    }

    void LoadBarriers()
    {
        List<BarrierData> loadedBarriers = BarrierData.LoadBarriers();
        foreach (BarrierData barrierData in loadedBarriers)
        {
            Vector3 position = new Vector3(barrierData.position[0], barrierData.position[1], barrierData.position[2]);
            Quaternion rotation = Quaternion.Euler(barrierData.rotation[0], barrierData.rotation[1], barrierData.rotation[2]);
            GameObject barrier = Instantiate(barrierPrefab, position, rotation);
            allBarriers.Add(barrier); // Add the barrier to the list
        }
    }
}
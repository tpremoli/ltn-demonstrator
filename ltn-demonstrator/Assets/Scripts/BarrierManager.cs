using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    public GameObject barrierPrefab; // Prefab for the barrier
    public List<GameObject> allBarriers; 

    public void LoadBarriers()
    {
        // Remove old barriers
        foreach (GameObject barrier in allBarriers)
        {
            Destroy(barrier);
        }
        allBarriers.Clear();

        // Load new barriers
        List<BarrierData> barrierDataList = BarrierData.LoadBarriers();
        foreach (BarrierData barrierData in barrierDataList)
        {
            GameObject newBarrier = Instantiate(barrierPrefab);
            newBarrier.transform.position = new Vector3(barrierData.position[0], barrierData.position[1], barrierData.position[2]);
            newBarrier.transform.rotation = Quaternion.Euler(barrierData.rotation[0], barrierData.rotation[1], barrierData.rotation[2]);
            allBarriers.Add(newBarrier);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{

    public GameObject barrierPrefab; // Prefab for the barrier
    public List<GameObject> allBarriers;

    public bool ShouldLoadBarriers = true;


    public static BarrierManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (ShouldLoadBarriers)
            {
                LoadBarriers();
            }
        }
    }


    public void LoadBarriers()
    {
        Debug.Log("Loading...");
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
            newBarrier.transform.parent = transform;
            allBarriers.Add(newBarrier);
        }
    }

    public void AddBarrier(Vector3 position)
    {
        GameObject newBarrier = Instantiate(barrierPrefab);
        newBarrier.transform.position = position;
        newBarrier.transform.parent = transform;
        allBarriers.Add(newBarrier);
    }
}
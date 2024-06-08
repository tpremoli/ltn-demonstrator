using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierManager : MonoBehaviour
{

    /*
    Current Edits in this branch:
    - Added a public field for the dropdown
    - Added a list of barrier prefabs
    - Added a public field for the barrier prefab
    - Added a list of all barriers
    */
    // Add a public field for the dropdown
    // The parent transform of the toggles
    public GameObject barrierPrefab; // Prefab for the barrier
    public GameObject blockAllPrefab;
    public GameObject blockAllMotorVehiclesPrefab;
    public GameObject blockHeavyTrafficPrefab;
    public GameObject busOnlyPrefab;
    public GameObject busAndTaxiOnlyPrefab;
    public List<GameObject> allBarriers;

    public bool loadBarriersFromSave;

    public static BarrierManager Instance { get; private set; }

    // List of different barrier prefabs
    public Dictionary<BarrierType, GameObject> barrierPrefabs = new Dictionary<BarrierType, GameObject>();


    private void Start()
    {
    }

    void Update()
    {
        // force reload barriers
        if (Input.GetKeyDown(KeyCode.U))
        {
            Physics.SyncTransforms();
            Debug.LogWarning("Reloading barriers!");
            RecalcBarriersOnEdges();
        }
        //Debug.Log("barrierPrefabs ", barrierPrefabs.Count);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (loadBarriersFromSave)
        {
            LoadBarriersFromSave();
        }
    }

    public void LoadBarriersFromSave()
    {
        Debug.Log("Loading...");
        // Remove old barriers
        foreach (GameObject barrier in allBarriers)
        {
            Destroy(barrier);
        }
        allBarriers.Clear();

        barrierPrefabs.Add(BarrierType.BlockAll, blockAllPrefab);
        barrierPrefabs.Add(BarrierType.BlockAllMotorVehicles, blockAllMotorVehiclesPrefab);
        barrierPrefabs.Add(BarrierType.BlockHeavyTraffic, blockHeavyTrafficPrefab);
        barrierPrefabs.Add(BarrierType.BusOnly, busOnlyPrefab);
        barrierPrefabs.Add(BarrierType.BusAndTaxiOnly, busAndTaxiOnlyPrefab);

        // Load new barriers
        List<BarrierData> barrierDataList = BarrierData.LoadBarriers();
        foreach (BarrierData barrierData in barrierDataList)
        {
            Vector3 pos = new Vector3(barrierData.position[0], barrierData.position[1], barrierData.position[2]);

            AddBarrier(pos, barrierData.type);
        }

        // this essentially reloads colliders so we can use them to generate barriers etc.
        // this is not efficient at all. HOWEVER, it is only called once on load.
        Physics.SyncTransforms();
        RecalcBarriersOnEdges();
    }

    public void AddBarrier(Vector3 position, BarrierType selectedBarrierType)
    {
        GameObject newBarrier = Instantiate(barrierPrefabs[selectedBarrierType], position, Quaternion.identity);

        newBarrier.GetComponent<Barrier>().BarrierType = selectedBarrierType;

        newBarrier.transform.Rotate(0, 90, 0);

        newBarrier.transform.name = selectedBarrierType.ToString();

        newBarrier.transform.parent = this.transform;

        // Rotate the barrier on the y axis 
        Graph graph = Graph.Instance;

        Edge closestEdge = graph.getClosetRoadEdge(position);
        Vector3 closestPointOnEdge = closestEdge.GetClosestPoint(position);
        Vector3 directionFromClosestPointToBarrier = newBarrier.transform.position - closestPointOnEdge; // Calculate direction vector

        if (directionFromClosestPointToBarrier != Vector3.zero)
        {
            // Normalize the direction vector
            Vector3 normalizedDirection = directionFromClosestPointToBarrier.normalized;

            // rotate barrier horizontal to the road
            newBarrier.transform.rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);

            // rotate 90 degrees
            newBarrier.transform.Rotate(0, 90, 0);

            // Set the barrier's position to this new position
            newBarrier.transform.position = position;
        }

        // Add the barrier to the list of all barriers
        allBarriers.Add(newBarrier);
    }



    public void RecalcBarriersOnEdges()
    {
        Graph graph = Graph.Instance;

        Debug.Log("Recalculating barriers on edges");

        // this is not efficient at all.
        foreach (Edge edge in graph.GetAllEdges())
        {
            edge.CheckBarriers();
        }
    }


}
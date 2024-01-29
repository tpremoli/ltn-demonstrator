using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{

    public Edge closestRoadEdge;
    public Vector3 closestPointOnRoadEdge;
    private Graph graph;
    private Waypoint startWaypointLane;

    public Waypoint endWaypoint;
    public GameObject barrierPrefab; // Prefab for the barrier
    public List<GameObject> allBarriers;

    public bool loadBarriersFromSave;


    public static BarrierManager Instance { get; private set; }

    void Update()
    {   
        // force reload barriers
        if (Input.GetKeyDown(KeyCode.U))
        {
            RecalcBarriersOnEdges();
        }
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

        // Load new barriers
        List<BarrierData> barrierDataList = BarrierData.LoadBarriers();
        foreach (BarrierData barrierData in barrierDataList)
        {
            GameObject newBarrier = Instantiate(barrierPrefab);
            newBarrier.transform.position = new Vector3(barrierData.position[0], barrierData.position[1], barrierData.position[2]);
            newBarrier.transform.rotation = Quaternion.Euler(barrierData.rotation[0], barrierData.rotation[1], barrierData.rotation[2]);
            newBarrier.transform.parent = transform;
            allBarriers.Add(newBarrier);

            // this essentially reloads colliders so we can use them to generate barriers etc.
            // this is not efficient at all. HOWEVER, it is only called once on load.
            Physics.SyncTransforms();
        }
    }

    public void AddBarrier(Vector3 position)
    {
        GameObject newBarrier = Instantiate(barrierPrefab, position, Quaternion.identity);
        newBarrier.transform.Rotate(0, 90, 0);
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
        foreach (Edge edge in graph.edges)
        {
            edge.RecheckBarriers();
        }

    }
}
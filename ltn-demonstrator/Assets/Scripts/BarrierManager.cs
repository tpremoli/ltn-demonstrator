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

    public bool useSave;


    public static BarrierManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (useSave)
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
        }
    }

    public void AddBarrier(Vector3 position)
    {
        GameObject newBarrier = Instantiate(barrierPrefab, position, Quaternion.identity);
        newBarrier.transform.Rotate(0, 90, 0); 
        // Rotate the barrier on the y axis 
        Graph tempGraph = GameObject.Find("Graph").GetComponent<Graph>();

        Edge closestEdge = tempGraph.getClosetRoadEdge(position);
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

    void RecalcBarriersOnEdges()
    {
        Graph graph = Graph.Instance;

        Debug.Log("Recalculating barriers on edges");

        foreach (Edge edge in graph.edges)
        {
            edge.barrier = edge.getBarrierInPath();

            Debug.Log("Edge between " + edge.startWaypoint.name + " and " + edge.endWaypoint.name + " has barrier " + edge.barrier);

            edge.isBarricated = edge.barrier != null;
            edge.barrierLocation = edge.barrier != null ? edge.convertToPositionAlongEdge(edge.barrier.transform.position) : -1f;

            if (edge.isBarricated)
            {
                Debug.Log("Edge between " + edge.startWaypoint.name + " and " + edge.endWaypoint.name + " is barricaded at " + edge.barrierLocation);
            }

        }

    }
}
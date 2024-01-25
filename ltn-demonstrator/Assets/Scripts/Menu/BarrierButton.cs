using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class BarrierButton : MonoBehaviour
{
    public GameObject barrierPrefab;
    public TextMeshProUGUI instructionText;
    private bool SpawnBarrier = false;
    private bool deleteMode = false;

    Transform barrierParent;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public BarrierManager barrierManager;
    public Edge closestRoadEdge;
    public Vector3 closestPointOnRoadEdge;
    private Graph graph;
    private Waypoint startWaypointLane;

    public Waypoint endWaypoint;


    void Start()
    {
        barrierManager = BarrierManager.Instance;
        if (barrierManager == null)
        {
            Debug.LogError("No BarrierManager found assigned to the BarrierButton.");
        }
    }

    public void SaveGame()
    {
        if (barrierManager != null)
        {
            List<Barrier> barriers = new List<Barrier>();
            foreach (GameObject gameObject in barrierManager.allBarriers)
            {
                Barrier barrier = gameObject.GetComponent<Barrier>();
                if (barrier != null)
                {
                    barriers.Add(barrier);
                }
            }
            SaveSystem.SaveBarriers(barriers);
            Debug.Log("Game Saved");
        }
        else
        {
            Debug.LogError("No BarrierManager found in the scene.");
        }
    }

    public void DeleteABarrier()
    {
        instructionText.text = "Click on desired barrier to delete";
        deleteMode = true;
    }

    public void DeleteSave()
    {
        foreach (GameObject barrierObject in barrierManager.allBarriers.ToArray())
        {
            barrierManager.allBarriers.Remove(barrierObject);
            Destroy(barrierObject);
        }

        Debug.Log("Deleted Barriers");
        SaveGame();
    }

    public void OnClick()
    {
        instructionText.text = "Click on desired barrier location";
        SpawnBarrier = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (SpawnBarrier && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 worldPosition = hit.point;
                Debug.Log("Barrier created at " + worldPosition);
                if (barrierManager != null)
                {
                    this.graph = GameObject.Find("Graph").GetComponent<Graph>();
                    this.closestRoadEdge = graph.getClosetRoadEdge(this.transform.position);
                    this.closestPointOnRoadEdge = closestRoadEdge.GetClosestPoint(this.transform.position);
                    Debug.Log("Closest Road Edge: " + closestRoadEdge.position + " Closest Point on Road Edge: " + closestPointOnRoadEdge);
                    Debug.Log("Direction of the Closest Road Edge: " + closestRoadEdge.direction);
                    //Edge nearestEdge = barrierManager.GetNearestEdge(worldPosition);
                    //Debug.Log("Nearest edge to worldPosition is at " + nearestEdge.position + " with direction " + nearestEdge.edgeDirection);

                    Debug.Log("Barrier List size: " + barrierManager.allBarriers.Count);
                    barrierManager.AddBarrier(worldPosition);
                    SpawnBarrier = false;
                }
                else
                {
                    Debug.LogError("No BarrierManager found in the scene.");
                }
            }
        } // This is the missing closing brace

        if (deleteMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Barrier hitBarrier = hit.transform.GetComponent<Barrier>();
                if (hitBarrier != null)
                {
                    barrierManager.allBarriers.Remove(hit.transform.gameObject);
                    Destroy(hit.transform.gameObject);
                    SaveGame();
                    deleteMode = false;
                }
            }
        }
    }
    /*
    public static void PositionBarrier()
    {
        Graph tempGraph = GameObject.Find("Graph").GetComponent<Graph>();

        Edge closestEdge = tempGraph.getClosetRoadEdge(this.transform.position);
        Vector3 closestPointOnEdge = closestEdge.GetClosestPoint(this.transform.position);
        Vector3 directionFromClosestPointToBarrier = barrier.transform.position - closestPointOnEdge; // Calculate direction vector

        if (directionFromClosestPointToBarrier != Vector3.zero)
        {
            // Normalize the direction vector
            Vector3 normalizedDirection = directionFromClosestPointToBarrier.normalized;

            // rotate barrier horizontal to the road
            barrier.transform.rotation = Quaternion.LookRotation(normalizedDirection.normalized, Vector3.up);

            // Set the barrier's position to this new position
            barrier.transform.position = newPosition;
        }
    

        // mark scene as dirty so it saves
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
    */
}
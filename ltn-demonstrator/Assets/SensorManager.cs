using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorManager : MonoBehaviour
{
    public GameObject sensorPrefab;
    public List<GameObject> allSensors;
    public static SensorManager Instance { get; private set; }

    public bool loadSensorsFromSave;

    void Start()
    {
        allSensors = new List<GameObject>();
    }

    void Update()
    {   
        // force reload sensors
        if (Input.GetKeyDown(KeyCode.U))
        {
            RecalcSensorsOnEdges();
        }
        //Debug.Log("sensorPrefabs ", sensorPrefabs.Count);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (loadSensorsFromSave)
        {
            LoadSensorsFromSave();
        }
    }

    public void LoadSensorsFromSave()
    {
        Debug.Log("Loading...");
        // Remove old sensors
        foreach (GameObject sensor in allSensors)
        {
            Destroy(sensor);
        }
        allSensors.Clear();

        // Load new sensors
        List<SensorData> sensorDataList = SensorData.LoadSensors();
        foreach (SensorData sensorData in sensorDataList)
        {
            GameObject newSensor = Instantiate(sensorPrefab);
            newSensor.transform.position = new Vector3(sensorData.position[0], sensorData.position[1], sensorData.position[2]);
            newSensor.transform.rotation = Quaternion.Euler(sensorData.rotation[0], sensorData.rotation[1], sensorData.rotation[2]);
            newSensor.transform.parent = transform;
            allSensors.Add(newSensor);

            // this essentially reloads colliders so we can use them to generate sensors etc.
            // this is not efficient at all. HOWEVER, it is only called once on load.
            Physics.SyncTransforms();
        }
    }

    public void AddSensor(Vector3 position)
    {
        GameObject newSensor = Instantiate(sensorPrefab, position, Quaternion.identity);

        newSensor.transform.Rotate(0, 90, 0);
        // Rotate the sensor on the y axis 
        Graph graph = Graph.Instance;

        Edge closestEdge = graph.getClosetRoadEdge(position);
        Vector3 closestPointOnEdge = closestEdge.GetClosestPoint(position);
        Vector3 directionFromClosestPointToSensor = newSensor.transform.position - closestPointOnEdge; // Calculate direction vector

        if (directionFromClosestPointToSensor != Vector3.zero)
        {
            // Normalize the direction vector
            Vector3 normalizedDirection = directionFromClosestPointToSensor.normalized;

            // rotate sensor horizontal to the road
            newSensor.transform.rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);

            // rotate 90 degrees
            newSensor.transform.Rotate(0, 90, 0);

            // Set the sensor's position to this new position
            newSensor.transform.position = position;
        }

        // Add the sensor to the list of all sensors
        allSensors.Add(newSensor);
        
    }



    
    public void RecalcSensorsOnEdges()
        {
            Graph graph = Graph.Instance;

            Debug.Log("Recalculating sensors on edges");

            // this is not efficient at all.
            foreach (Edge edge in graph.edges)
            {
                edge.RecheckSensors();
            }

        }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensorManager : MonoBehaviour
{
    public GameObject sensorPrefab;
    public List<GameObject> allSensors;
    public static SensorManager Instance { get; private set; }

    public Text instructionText;
    public void LoadSensorsFromSave()
    {
        Debug.Log("Loading...");
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
            Physics.SyncTransforms();
        }
    }
    public void SaveSensors()
    {
        List<Sensor> sensors = new List<Sensor>();
        foreach (GameObject gameObject in allSensors)
        {
            Sensor sensor = gameObject.GetComponent<Sensor>();
            if (sensor != null)
            {
                sensors.Add(sensor);
            }
        }
        SaveSystem.SaveSensors(sensors);
        Debug.Log("Sensors Saved");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        LoadSensorsFromSave();
    }

    void Start()
    {
        allSensors = new List<GameObject>();
    }

    public void AddSensor(Vector3 position, Quaternion rotation)
    {
        GameObject newSensor = Instantiate(sensorPrefab);
        newSensor.transform.position = position;
        newSensor.transform.rotation = rotation;
        newSensor.transform.parent = transform;
        allSensors.Add(newSensor);
    }

    public void OnDeleteSensorPressed()
    {
        instructionText.text = "Click on desired sensor to delete";
        deleteMode = true;
    }

    public void OnDeleteSavePressed()
    {
        foreach (GameObject sensorObject in allSensors.ToArray())
        {
            allSensors.Remove(sensorObject);
            Destroy(sensorObject);
        }
        Debug.Log("Deleted Sensors");
        SaveSensors();
    }

    public void OnAddSensorPressed()
    {
        instructionText.text = "Click on the map to add a sensor";
        spawnSensor = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // Add this line

[System.Serializable]
public class SensorsContainer
{
    // container for list of sensor data
    public List<SensorData> sensors;
}

[System.Serializable]
public class SensorData
{
    public float[] position;
    public float[] rotation;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public SensorData(Sensor sensor)
    {
        position = new float[] { sensor.transform.position.x, sensor.transform.position.y, sensor.transform.position.z };
        rotation = new float[] { sensor.transform.rotation.eulerAngles.x, sensor.transform.rotation.eulerAngles.y, sensor.transform.rotation.eulerAngles.z };
    }

    public static List<SensorData> LoadSensors()
    {
        string json = File.ReadAllText(SAVE_FOLDER + "sensor_save.json");
        SensorsContainer data = JsonUtility.FromJson<SensorsContainer>(json);

        if (data == null)
        {
            // If the file is empty, create a new SensorsContainer
            // with an empty list of sensors
            data = new SensorsContainer { sensors = new List<SensorData>() };
        }
        else if (data.sensors == null)
        {
            // If the file is not empty but the list of sensors is null,
            // create a new list of sensors
            data.sensors = new List<SensorData>();
        }

        return data.sensors;
    }
}
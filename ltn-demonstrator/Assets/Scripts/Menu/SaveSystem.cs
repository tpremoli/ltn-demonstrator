using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    [System.Serializable]
    public class BarriersContainer
    {
        public List<BarrierData> barriers;
    }

    [System.Serializable]
    public class SensorsContainer
    {
        public List<SensorData> sensors;
    }

    public static void SaveSensors(List<Sensor> sensors)
    {
        
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
            Debug.Log("Created directory " + SAVE_FOLDER + "at" + Application.dataPath);
        }

        Debug.Log("Saving to " + SAVE_FOLDER + "sensor_save.json" + "at" + Application.dataPath);

        List<SensorData> sensorDataList = new List<SensorData>();
        foreach (Sensor sensor in sensors)
        {
            sensorDataList.Add(new SensorData(sensor));
        }

        SensorsContainer container = new SensorsContainer();
        container.sensors = sensorDataList;
        string json = JsonUtility.ToJson(container);

        File.WriteAllText(SAVE_FOLDER + "sensor_save.json", json);
    }

    public static void SaveBarriers(List<Barrier> barriers)
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
            Debug.Log("Created directory " + SAVE_FOLDER + "at" + Application.dataPath);
        }

        Debug.Log("Saving to " + SAVE_FOLDER + "save.json" + "at" + Application.dataPath);

        List<BarrierData> barrierDataList = new List<BarrierData>();
        foreach (Barrier barrier in barriers)
        {
            barrierDataList.Add(new BarrierData(barrier));
        }

        BarriersContainer container = new BarriersContainer();
        container.barriers = barrierDataList;
        string json = JsonUtility.ToJson(container);

        File.WriteAllText(SAVE_FOLDER + "save.json", json);
    }

    public static List<BarrierData> LoadBarriers()
    {
        if (File.Exists(SAVE_FOLDER + "save.json"))
        {
            string json = File.ReadAllText(SAVE_FOLDER + "save.json");
            var data = JsonUtility.FromJson<Root>(json);
            return data.barriers;
        }
        else
        {
            Debug.LogError("Save file not found in " + SAVE_FOLDER + "save.json");
            return null;
        }
    }

    public static List<SensorData> LoadSensors()
    {
        if (File.Exists(SAVE_FOLDER + "sensor_save.json"))
        {
            string json = File.ReadAllText(SAVE_FOLDER + "sensor_save.json");
            var data = JsonUtility.FromJson<SensorsContainer>(json);
            return data.sensors;
        }
        else
        {
            Debug.LogError("Save file not found in " + SAVE_FOLDER + "sensor_save.json");
            return null;
        }
    }

    [System.Serializable]
    private class Root
    {
        public List<BarrierData> barriers;
    }
}
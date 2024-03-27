using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // Add this line

[System.Serializable]
public class BarriersContainer
{
    public List<BarrierData> barriers;
}

[System.Serializable]
public class BarrierData
{
    public float[] position;
    public float[] rotation;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public BarrierData(Barrier barrier)
    {
        position = new float[] { barrier.transform.position.x, barrier.transform.position.y, barrier.transform.position.z };
        rotation = new float[] { barrier.transform.rotation.eulerAngles.x, barrier.transform.rotation.eulerAngles.y, barrier.transform.rotation.eulerAngles.z };
    }

    public static List<BarrierData> LoadBarriers()
    {
        string json = File.ReadAllText(SAVE_FOLDER + "save.json");
        BarriersContainer data = JsonUtility.FromJson<BarriersContainer>(json);
            
        if (data == null)
        {
            // If the file is empty, create a new BarriersContainer
            // with an empty list of barriers
            data = new BarriersContainer { barriers = new List<BarrierData>() };
        }
        else if (data.barriers == null)
        {
            // If the file is not empty but the list of barriers is null,
            // create a new list of barriers
            data.barriers = new List<BarrierData>();
        }
            
        return data.barriers;
    }
}
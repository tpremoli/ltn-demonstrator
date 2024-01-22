using System.Collections.Generic;
using UnityEngine;


public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    // list to store all PathData instances.
    public TravellerManager tm;
    public static List<PathData> allPathData { get; private set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make the TravellerManager's GameObject persistent
            allPathData = new List<PathData>(); // Initialize the list.
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance is active.
        }
    }
    // Method to add PathData to the list.
    public void AddPathData(PathData pathData)
    {
        allPathData.Add(pathData);
    }


    public void Update(){
            if (TravellerManager.Instance.noOfTravellers >= 10) // Assuming noOfTravellers is a public field or property
            {
                // End Simulation - change to next scene
                Debug.Log("SIMULATION ENDED");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
            }

    public void UpdateEndTime(int id) {
        foreach (var pathData in allPathData)
        {
            if (pathData.ID == id)
            {
                pathData.endTime = Time.frameCount; 
            }
        }
    }
    
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;



public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    // list to store all PathData instances.
    public TravellerManager tm;
    public static List<PathData> allPathData { get; private set; }
    //text for stats in the statistical measures screen
    public TMP_Text statsText;
    public const int TERMINATION_CRITERIA = 10;
    private int finishedPaths;
    private bool endSim;
    

    //-------------------------------INBUILT FUNCTION EXTENSIONS---------------------------------------------------------------------------------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make the TravellerManager's GameObject persistent
            allPathData = new List<PathData>(); // Initialize the list.
            //initialise termination criteria vars
            finishedPaths = 0; 
            endSim = false; 
            //speed up simulation
            Time.timeScale = 5;
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance is active.
        }
    }

    public void Update()
    {
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "ProperMapSceneForStats")
        {
            if (endSim == true) // Assuming noOfTravellers is a public field or property
            {
                // End Simulation - change to next scene
                Debug.Log("SIMULATION ENDED");
                //change scene
                SceneManager.LoadScene("StatisticsScene");

                //#if UNITY_EDITOR
                //    UnityEditor.EditorApplication.isPlaying = false;
                //#else
                //    Application.Quit();
                //#endif
            }
        }
        if (currentSceneName == "StatisticsScene") {
            //code to update stats
            //acquire the statsText Object

            //Debug.Log("active");
            //UpdateStatsText();

            //get path data for test
        }
    }
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "StatisticsScene") {
            statsText = GameObject.Find("Body").GetComponent<TMP_Text>();
            if (statsText == null) {
                Debug.LogError("Failed to find the TMP_Text component on 'Body'.");
            } else {
                Debug.Log("Should be updating stats as we speak");
                //Clean the data
                PrunePathData();
                // Now you can update the statsText with your data.
                UpdateTextWithStatistics();
                
            }
        }
    }


    //-------------------------------------DIAGNOSTICS AND ERROR HANDLING----------------------------------------------------------------------------
    //function to check data is correct and learn about readability
    public string ReadData(PathData pd)
    {
        if (pd == null)
        {
            return "PathData is null.";
        }

        string pathString = "Path Edges: ";
        if (pd.path != null && pd.path.Count > 0)
        {
            // Assuming your Edge class has a method ToString() that describes the edge well,
            // otherwise you might just print out some identifier of the edge.
            foreach (Edge edge in pd.path)
            {
                //pathString += edge.ToString() + "; ";
                pathString += "e, ";
            }
        }
        else
        {
            pathString += "No path defined.";
        }

        // Construct the data string
        string dataString = $"PathData ID: {pd.ID}\n" +
                            $"{pathString}\n" +
                            $"Start Time: {pd.startTime}\n" +
                            $"End Time: {pd.endTime}\n" +
                            $"Route Changed: {(pd.routeChange ? "Yes" : "No")}\n" +
                            $"Traveller Type: {pd.travellerType}";

        return dataString;
    }

    public void BUGFIXincrementFinishedPaths() {
        finishedPaths++;
    }


    //-------------------------------------DATA HANDLING---------------------------------------------------------------------------------------------
    //Prune all incomplete data entries
    private void PrunePathData()
    {
        for (int i = allPathData.Count - 1; i >= 0; i--)
        {
            //check id field
            if (allPathData[i].ID < 0 || 
                allPathData[i].startTime < 0 || 
                allPathData[i].endTime < 1 || 
                !(allPathData[i].routeChange == false || allPathData[i].routeChange == true) || 
                !Enum.IsDefined(typeof(ModeOfTransport), allPathData[i].travellerType))
            {
                allPathData.RemoveAt(i);
            }
            //add functionality to check validity of path
        }
    }

    private void UpdateTextWithStatistics() {
        // Assuming you have a method to format your statistics data into a string
        //string stats = GetFormattedStatistics();
        string stats = "Changes";

        statsText.text = stats;

        //do for each statisic, send string to TMP_Object
        string finalString = "";
        //Total time spent travelling
        string totalTravelTime = TotalTravelTime();
        string totalNoOfTravellers = TotalNumberOfTravellers();
        string averageTravVelo = AverageTravellerVelocity();
        string rateOfDeviation = RateOfDeviation();
        finalString = $"Number of travellers: {totalNoOfTravellers} \nTotal travel time: {totalTravelTime} seconds\nAverage traveller velocity: {averageTravVelo} \nRate of deviation from original path: {rateOfDeviation} \nAverage rate of pollution: N/A \nTotal pollution: N/A ";
        //Set the TMP object to the stats we calc
        statsText.text = finalString;
        
    }

    // Method to add PathData to the list.
    public void AddPathData(PathData pathData)
    {
        allPathData.Add(pathData);
    }

    public void RecieveEndTime (int ID) {
        UpdateEndTime(ID);
        finishedPaths++;
        Debug.Log($"finished Paths = {finishedPaths}, term = {TERMINATION_CRITERIA}, num of spawned Travellers = {TravellerManager.Instance.noOfTravellers}");
        if (finishedPaths >= TERMINATION_CRITERIA) {
            Debug.Log("ending simulation");
            endSim = true;
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


    //---------------------------------STATISTICAL MEASURES------------------------------------------------------------------------------------------

    //Total travel time spent
    private string TotalTravelTime ()
    {
        float totalTravelTime = 0;
        foreach (var pathData in allPathData) {
            totalTravelTime += pathData.endTime - pathData.startTime;
            //Debug.Log($"{pathData.endTime} - {pathData.startTime} = {pathData.endTime - pathData.startTime}");
        }
        // convert to seconds
        totalTravelTime = totalTravelTime*Time.deltaTime*0.5;
        return totalTravelTime.ToString() ;
    }

    //Total Number of Travellers
    private string TotalNumberOfTravellers () 
    {
        return allPathData.Count.ToString();
    }

    //Average traveller velocity
private string AverageTravellerVelocity() 
{
    float totalTime;
    float totalDistance = 0;
    int numberOfTravellers;
    float.TryParse(TotalTravelTime(), out totalTime);
    int.TryParse(TotalNumberOfTravellers(), out numberOfTravellers);

    foreach (PathData pd in allPathData) 
    {
        if (pd.path == null)
        {
            Debug.Log(Time.deltaTime);
            Debug.LogWarning("PathData path is null");
            continue; // Skip this PathData as its path is null
        }

        foreach (Edge e in pd.path)
        {
            if (e == null)
            {
                Debug.LogWarning("Edge in path is null");
                continue; // Skip this Edge as it is null
            }

            totalDistance += e.length;
        }
    }

    if (totalTime * numberOfTravellers == 0)
    {
        return "0"; // Avoid division by zero
    }

    return (totalDistance / (totalTime * numberOfTravellers)).ToString();
}



    //Rate of deviation from original path
    private string RateOfDeviation ()
    {
        int deviated = 0;
        foreach (PathData pd in allPathData) 
        {
            if (pd.routeChange == true)
            {
                deviated++;
            }
        }
        return (deviated/allPathData.Count).ToString();
    }

    //Rate of pollution - uses arbitrary values for now
    


    //Total Pollution




}

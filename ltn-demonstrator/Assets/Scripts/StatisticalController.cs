using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.IO;





public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    // list to store all PathData instances.
    public TravellerManager tm;
    public static List<PathData> allPathData { get; private set; }

    // real time data
    public static List<PathData> RealTimePathData { get; private set; }

    public static List<PathData> finishPathData { get; private set; }
    //text for stats in the statistical measures screen
    public TMP_Text statsText;
    public const int TERMINATION_CRITERIA = 10;
    private int finishedPaths;
    private bool endSim;

    public TextMeshProUGUI StatisticsText;
    
    
    // --------------------------------------SERIALISATION ASSETS------------------------------------------
    //Used for loading and saving data
    [System.Serializable]
    private class Serialization<T>
    {
        [SerializeField]
        List<T> items;

        public Serialization(List<T> items)
        {
            this.items = items;
        }

        public List<T> ToList()
        {
            return items;
        }
    }




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
        //--------------------------------REMOVE
        //--------------------------------------
        //--------------------------------------
        //--------------------------------------
        Time.timeScale = 10;
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        // fix this
        if (currentSceneName != "StatisticsScene")
        {
            if (endSim == true) // Assuming noOfTravellers is a public field or property
            {
                // End Simulation - change to next scene
                Debug.Log("SIMULATION ENDED");
                //Clean the data
                PrunePathData();
                Debug.Log("Pruned PathData");
                //serialise data
                SerialisePathDataSave();
                Debug.Log("Serialised PathData");
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

        // update StatisticsText in real time
        TextMeshProUGUI StatisticsText = GameObject.Find("Body").GetComponent<TextMeshProUGUI>();
        RealTime_UpdateTextWithStatistics();

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
                // update the statsText with your data.
                UpdateTextWithStatistics();

                //serialise pathdata
                //SerialisePathDataSave();
                
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
                !Enum.IsDefined(typeof(ModeOfTransport), allPathData[i].travellerType) ||
                allPathData[i].path == null)
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
        finalString = $"Number of travellers: {totalNoOfTravellers} \nTotal travel time: {totalTravelTime} seconds\nAverage traveller velocity: {averageTravVelo} spatial units/s\nRate of deviation from original path: {rateOfDeviation} \nAverage rate of pollution: N/A \nTotal pollution: N/A ";
        //Set the TMP object to the stats we calc
        statsText.text = finalString;
        
    }

    private void RealTime_UpdateTextWithStatistics() {
        // Assuming you have a method to format your statistics data into a string
        //string stats = GetFormattedStatistics();
        string stats = "Changes";

        StatisticsText.text = stats;

        //do for each statisic, send string to TMP_Object
        string finalString = "";
        //Total time spent travelling
        string totalTravelTime = TotalTravelTime();
        string totalNoOfTravellers = TotalNumberOfTravellers();
        string averageTravVelo = AverageTravellerVelocity();
        string rateOfDeviation = RateOfDeviation();
        finalString = $"Number of travellers: {totalNoOfTravellers} \nTotal travel time: {totalTravelTime} seconds\nAverage traveller velocity: {averageTravVelo} spatial units/s\nRate of deviation from original path: {rateOfDeviation} \nAverage rate of pollution: N/A \nTotal pollution: N/A ";
        //Set the TMP object to the stats we calc
        StatisticsText.text = finalString;
        
    }

    // Method to add PathData to the list.
    public void AddPathData(PathData pathData)
    {
        allPathData.Add(pathData);
    }

    public void RecieveEndTime (int ID, List<Edge> path) {
        UpdateEndTimeAndPath(ID, path);
        finishedPaths++;
        Debug.Log($"finished Paths = {finishedPaths}, term = {TERMINATION_CRITERIA}, num of spawned Travellers = {TravellerManager.Instance.noOfTravellers}");
        if (finishedPaths >= TERMINATION_CRITERIA) {
            Debug.Log("ending simulation");
            endSim = true;
        }
    }


    public void UpdateEndTimeAndPath(int id, List<Edge> path) { // stats of path
        foreach (var pathData in allPathData)
        {
            if (pathData.ID == id)
            {
                pathData.endTime = Time.frameCount; 
                pathData.path = path;
                if (path == null) {
                    Debug.LogError("path is null");
                }
            }
        }
    }



    // for each edge object in the simulation, convert to serialisable edge
    private List<SerialisableEdge> convertEdgeToSerialisable () {
        List <Edge> old_edges = new List<Edge>();
        List <SerialisableEdge> new_edges = new List<SerialisableEdge>();
        int ID_counter = 0;
        Debug.Log("converting edges");
        //get all edges, extract on necessary from allPathData
        foreach (var pathData in allPathData) {
            foreach (var edge in pathData.path) {
                //assign ID attribute to each edge arbitrarily
                if (edge.ID == -1) {
                    edge.ID = ID_counter;
                    ID_counter+=1;
                    old_edges.Add(edge);
                    //create serialisable edge from original edge
                    SerialisableEdge serial_edge = new SerialisableEdge(edge);
                    new_edges.Add(serial_edge);
                }
                else {
                    continue;

                }
            }
        }
        Debug.Log($"finished converting edges, length of new_edges = {new_edges.Count}");
        return new_edges;
    }



    //for each waypoint in the simulation, convert to a serialisable waypoint
    //---------------------------------------FIX-----------------------------------------------
    private List<SerialisableWaypoint> convertWaypointToSerialisable () {
        List <Waypoint> old_waypoints = new List<Waypoint>();
        List <SerialisableWaypoint> new_waypoints = new List<SerialisableWaypoint>();
        int ID_counter = 0;
        //get all waypoints, extract on necessary from allPathData
        foreach (var pathData in allPathData) {
            foreach (var edge in pathData.path) {
                //assign ID attribute to each edge arbitrarily
                if (edge.startWaypoint.ID == -1) {
                    edge.startWaypoint.ID = ID_counter;
                    ID_counter++;
                    old_waypoints.Add(edge.startWaypoint);
                    //create serialisable edge from original edge
                    SerialisableWaypoint serial_waypoint = new SerialisableWaypoint(edge.startWaypoint);
                    new_waypoints.Add(serial_waypoint);
                }
                else {
                    continue;
                }
                if (edge.endWaypoint.ID == -1) {
                    edge.endWaypoint.ID = ID_counter;
                    ID_counter++;
                    old_waypoints.Add(edge.endWaypoint);
                    //create serialisable edge from original edge
                    SerialisableWaypoint serial_waypoint = new SerialisableWaypoint(edge.endWaypoint);
                    new_waypoints.Add(serial_waypoint);
                }
                else {
                    continue;
                }   
            }
        }
        return new_waypoints;
    }




    //convert all pathData to serialisable pathdata
    public void SerialisePathDataSave (bool saveAsJson = false) {
        Debug.Log("BEGINNING SERIALISATION");
        //serialise the waypoints
        List<SerialisableWaypoint> allNewWaypoints = convertWaypointToSerialisable();
        Debug.LogError($"length of allNewWaypoints: {allNewWaypoints.Count}");
        //get the serialised versions of all edges
        List<SerialisableEdge> allNewEdges = convertEdgeToSerialisable();
        //for each edge in each path, create a serialisable edge list
        foreach (var pathData in allPathData) {
            //create a serialisable pathData
            List<SerialisableEdge> newEdges = new List<SerialisableEdge>();
            foreach (Edge edge in pathData.path) {
                //find equivilant edge in allNewEdges
                foreach (SerialisableEdge newEdge in allNewEdges) {
                    if (edge.ID == newEdge.ID) {
                        newEdges.Add(newEdge);
                        
                    }
                    else {
                        continue;
                    }
                }
            }
             //set each pd.serialisablePath to the list
            pathData.serialisablePath = newEdges;
        }
        Debug.Log($"Serialised all pathData, count is {allPathData.Count}");
        //save the allPathData list as a json file if paramater is true
        if (saveAsJson == true) {
            //save as json
            SaveToJson(allPathData, "pathData.json");
        }

    }

    //Save to Json
    public void SaveToJson<T>(List<T> objects, string filename)
    {
        string relativePath = "Saves/" + filename; // Relative path within the Assets directory
        string fullPath = Path.Combine(Application.dataPath, relativePath);
        
        string json = JsonUtility.ToJson(new Serialization<T>(objects));
        File.WriteAllText(fullPath, json);
    }


    //Load from Json
    public List<T> LoadFromJson<T>(string filename)
    {
        string relativePath = "Saves/" + filename; // Relative path within the Assets directory
        string fullPath = Path.Combine(Application.dataPath, relativePath);

        if (!File.Exists(fullPath))
        {
            return new List<T>(); // Return an empty list if the file doesn't exist
        }

        string json = File.ReadAllText(fullPath);
        Serialization<T> data = JsonUtility.FromJson<Serialization<T>>(json);
        return data.ToList();
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
        totalTravelTime = (float)(totalTravelTime*Time.deltaTime);
        return totalTravelTime.ToString() ;
    }

    //Total Number of Travellers
    private string TotalNumberOfTravellers () 
    {
        return allPathData.Count.ToString();
    }

    //Average traveller velocity - needs delta D conversion
    private string AverageTravellerVelocity() 
    {
        float totalTime;
        float totalDistance = 0;
        int numberOfTravellers;
        float.TryParse(TotalTravelTime(), out totalTime);
        int.TryParse(TotalNumberOfTravellers(), out numberOfTravellers);

        foreach (PathData pd in allPathData) 
        {
            
            if (pd.serialisablePath == null)
            {
                Debug.Log(Time.deltaTime);
                Debug.LogWarning("PathData path is null");
                continue; // Skip this PathData as its path is null
            }
            foreach (SerialisableEdge e in pd.serialisablePath)
            {
                if (e == null)
                {
                    Debug.LogWarning("Edge in path is null");
                    continue; // Skip this Edge as it is null
                }
                //Debug.Log($"e.length is {e.length}");
                totalDistance += e.length;
            }
        }
        if (totalTime * numberOfTravellers == 0)
        {
            return "0"; // Avoid division by zero
        }
        //Needs delta D conversion
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
    private string RateOfPollution ()
    {
        return "N/A";
    }


    //Total Pollution







    //-------------------------------HEATMAP GENERATION-------------------------------------------------------------------------------------------
    
    public void MakeHeatmap ()
    {

    }


    public void GetEdgeWeights () {
        //iterate through all pathdata serialisable paths
        foreach (PathData pd in allPathData) {
            foreach (SerialisableEdge e in pd.serialisablePath) {
                //increment pathdata weight
                e.weight++;
            }
        }
        //normalise weights
        foreach (PathData pd in allPathData) {
            foreach (SerialisableEdge e in pd.serialisablePath) {
                e.weight = e.weight / allPathData.Count;
            }
        }
    }


}




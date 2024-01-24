using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    // list to store all PathData instances.
    public TravellerManager tm;
    public static List<PathData> allPathData { get; private set; }
    //text for stats in the statistical measures screen
    public TMP_Text statsText;
    private int finishedPaths;
    private bool endSim;
    

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
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
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
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "BuildingScene")
        {
            if (TravellerManager.Instance.noOfTravellers >= 10) // Assuming noOfTravellers is a public field or property
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



    public void UpdateStatsText() {
        if (statsText.text != null) 
        {
            Debug.Log("kinda working");
            Debug.Log(allPathData[0].ID);
            //string pd = ReadData(allPathData[0]);
            //statsText.text = pd;
            statsText.text = "Heooollool";
            }
        else 
        {
            Debug.Log("Object not available");
        }
    }


    public void RecieveEndTime (int ID) {
        UpdateEndTime(ID);
        finishedPaths++;


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
                // Now you can update the statsText with your data.
                UpdateTextWithStatistics();
            }
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
        finalString = $"Total travel time: {totalTravelTime} \nAverage traveller velocity: N/A \nRate of deviation from original path: N/A \nAverage rate of pollution: N/A \nTotal pollution: N/A ";



        statsText.text = finalString;
        
    }



    

    //Total travel time spent
    private string TotalTravelTime (){
        float totalTravelTime = 0;
        foreach (var pathData in allPathData) {
            totalTravelTime += pathData.endTime - pathData.startTime;
            Debug.Log($"{pathData.endTime} - {pathData.startTime} = {pathData.endTime - pathData.startTime}");
        }
        return totalTravelTime.ToString();
    }


    //Average traveller velocity

    //Rate of deviation from original path

    //Rate of pollution - uses arbitrary values for now
    
}

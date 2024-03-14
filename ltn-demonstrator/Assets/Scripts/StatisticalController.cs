    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using TMPro;
    using System;
    using System.IO;
    using UnityEngine.UI;






    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager Instance { get; private set; }
        // list to store all PathData instances.
        public TravellerManager tm;
        public static List<PathData> allPathData { get; private set; }

        //Used to represent the edges and waypoints in the simulation
        public static List<SerialisableEdge> allEdges { get; private set; }
        public static List<SerialisableWaypoint> allWaypoints { get; private set; }

        //text for stats in the statistical measures screen
        public TMP_Text statsText;
        //loads a whitescreen object
        public GameObject whiteScreenPanel;
        public GameObject waypointPrefab;
        public GameObject linePrefab; 

        //public GameObject edgePrefab;

        public const int TERMINATION_CRITERIA = 10;
        private int finishedPaths;
        private bool endSim;
        private bool isHeatMapVisible = false;       //used to toggle the heatmap on and off
        

        
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
                    CreateSerialisableEdgesAndWaypoints();
                    Debug.LogError("SERIALISED EDGES AND WAYPOINTS");
                    //SerialisePathDataSave();
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
                    FindWhiteScreen();


                    
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

        public void UpdateEndTimeAndPath(int id, List<Edge> path) {
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


        //get all edges in the simulation
        private List<Edge> GetAllEdges() {
            if (Graph.Instance != null) {
                return Graph.Instance.edges;
            }
            return new List<Edge>();
        }

        //get all waypoints in the simulation
        private List<Waypoint> GetAllWaypoints() {
            if (Graph.Instance != null) {
                return Graph.Instance.waypoints;
            }
            return new List<Waypoint>();
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


        //converts a list of edges to a list of serialisable edges
        private List<SerialisableEdge> ConvertEdgeListToSerializable(List<Edge> edges) {
            List<SerialisableEdge> new_edges = new List<SerialisableEdge>();
            int ID_counter = 0;
            foreach (var edge in edges) {
                if (edge.ID == -1) {
                    edge.ID = ID_counter++;
                }
                // Assume SerialisableEdge has a constructor that takes an Edge object
                SerialisableEdge serial_edge = new SerialisableEdge(edge);
                //find corresponding serialisable waypoints in allwaypoints
                foreach (var waypoint in allWaypoints) {
                    if (waypoint.ID == edge.startWaypoint.ID) {
                        serial_edge.startWaypoint = waypoint;
                    }
                    if (waypoint.ID == edge.endWaypoint.ID) {
                        serial_edge.endWaypoint = waypoint;
                    }
                }
                new_edges.Add(serial_edge);

            }
            return new_edges;
        }


        //converts a list of waypoints to a list of serialisable waypoints
        private List<SerialisableWaypoint> ConvertWaypointsToSerializable(List<Waypoint> waypoints) {
            List<SerialisableWaypoint> new_waypoints = new List<SerialisableWaypoint>();
            int ID_counter = 0;
            foreach (var waypoint in waypoints) {
                if (waypoint.ID == -1) {
                    waypoint.ID = ID_counter++;
                }
                // Assume SerialisableWaypoint has a constructor that takes a Waypoint object
                SerialisableWaypoint serial_waypoint = new SerialisableWaypoint(waypoint);
                new_waypoints.Add(serial_waypoint);
            }
            return new_waypoints;
        }






        //convert all pathData to serialisable pathdata
        public void SerialisePathDataSave (bool saveAsJson = false) {
            Debug.Log("BEGINNING SERIALISATION");
            //serialise the waypoints
            List<SerialisableWaypoint> allNewWaypoints = allWaypoints;
            Debug.LogError($"length of allNewWaypoints: {allNewWaypoints.Count}");
            //get the serialised versions of all edges
            List<SerialisableEdge> allNewEdges = allEdges;
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



        public void CreateSerialisableEdgesAndWaypoints() {
            allWaypoints = ConvertWaypointsToSerializable(GetAllWaypoints());
            allEdges = ConvertEdgeListToSerializable(GetAllEdges());
            Debug.LogError($"allEdges length: {allEdges.Count}, allWaypoints length: {allWaypoints.Count}");
        }

        //-------------------------------------HEATMAP FUNCTIONS---------------------------------------------------------------------------------------

        public void ShowWhiteScreen()
        {
            if (whiteScreenPanel != null)
            {
                whiteScreenPanel.SetActive(true);
            }
        }

        public void HideWhiteScreen()
        {
            if (whiteScreenPanel != null)
            {
                whiteScreenPanel.SetActive(false);
            }
        }


        public void FindWhiteScreen()
        {
            // find whiteScreenPanel
            if (whiteScreenPanel == null)
                {
                    Debug.LogWarning("WhiteScreen Being searched for ");
                    whiteScreenPanel = GameObject.Find("WhiteScreen");
                    if (whiteScreenPanel == null)
                    {
                        Debug.LogError("Failed to find the WhiteScreen GameObject.");
                    }
                }
                HideWhiteScreen();
                //ShowWhiteScreen();

            Button heatmapToggleButton = GameObject.Find("Heatmap Toggle").GetComponent<Button>(); // Replace with your actual button name
            if (heatmapToggleButton != null) {
                heatmapToggleButton.onClick.RemoveAllListeners(); // Remove existing listeners to prevent stacking if scene is reloaded
                heatmapToggleButton.onClick.AddListener(ToggleHeatMap);
                Debug.Log("Added listener to Heatmap Toggle Button.");
            } else {
                Debug.LogError("Failed to find the Heatmap Toggle Button GameObject.");
            }
        }


        public void CalculateTransform(List<SerialisableWaypoint> waypoints, float margin = 0.9f)
        {
            // get the dimensions of the panel
            GameObject panelGameObject = GameObject.Find("WhiteScreen");
            RectTransform panelRectTransform = panelGameObject.GetComponent<RectTransform>();
            Vector2 panelSize = panelRectTransform.rect.size;

            Debug.LogError($" panelSize = {panelSize}");

            // calculate the scale factor for the x and y coordinates
            float minx = float.MaxValue;
            float maxx = float.MinValue;
            float minz = float.MaxValue;
            float maxz = float.MinValue;

            foreach (var waypoint in waypoints)
            {

                if (waypoint.x < minx)
                {
                    minx = waypoint.x;
                }
                if (waypoint.x > maxx)
                {
                    maxx = waypoint.x;
                }
                if (waypoint.z < minz)
                {
                    minz = waypoint.z;
                }
                if (waypoint.z > maxz)
                {
                    maxz = waypoint.z;
                }
            }

            Debug.Log($"minx = {minx}, maxx = {maxx}, minz = {minz}, maxz = {maxz}");

            float xStretch = (panelSize.x / (maxx - minx))*margin;
            float zStretch = (panelSize.y / (maxz - minz))*margin;

            Debug.Log($"xStretch = {xStretch}, zStretch = {zStretch}");

            //
            foreach (var waypoint in waypoints)
            {
                waypoint.x = ((waypoint.x - minx) / (maxx - minx) * panelSize.x * margin) + (panelSize.x * (1 - margin) / 2);
                waypoint.z = ((waypoint.z - minz) / (maxz - minz) * panelSize.y * margin) + (panelSize.y * (1 - margin) / 2);

            }
        }




        public void DrawWaypoints(List<SerialisableWaypoint> waypoints)
        {
            //Transform waypoints so that they are correctly oriented
            CalculateTransform(waypoints);
            for (int i = 0; i < waypoints.Count; i++)
            {
                //draw the waypoint
                Debug.Log($"Waypoint {waypoints[i].ID} drawn at {waypoints[i].x}, {waypoints[i].y}, {waypoints[i].z}");
                //create gameObject in unity for each waypoint and place it at the coordinates
                // Create the waypoint position from the serialized data
                Vector3 waypointPosition = new Vector3(waypoints[i].x, waypoints[i].z, -3); //(x,y,z)
                // Instantiate the waypoint prefab at this position
                Instantiate(waypointPrefab, waypointPosition, Quaternion.identity);
                Debug.Log("Spawned");


            }

        }


        public void DrawEdges(List<SerialisableEdge> edges)
        {
            Debug.LogError("Drawing Edges");
            for (int i = 0; i < edges.Count; i++)
            {
                //draw the edge
                Debug.Log($"Edge {edges[i].ID} drawn from {edges[i].startWaypoint.ID} to {edges[i].endWaypoint.ID}");

                Vector3 startPoint = new Vector3(edges[i].startWaypoint.x, edges[i].startWaypoint.z, -1);
                Vector3 endPoint = new Vector3(edges[i].endWaypoint.x, edges[i].endWaypoint.z, -1);

                DrawLine(startPoint, endPoint);   
                Debug.Log($"Edge weight is {edges[i].weight}");
            }
        }



        public void DrawLine(Vector3 start, Vector3 end)
        {   
            float weight = 15f; // This is your existing weight value
            float uniformWidth = Mathf.Lerp(0.1f, 0.5f, weight*100); // Calculate the uniform width based on weight

            //translate the start and end points to the correct position, increase z by 5
            start.z = -3;
            end.z = -3;


            GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

            // Set the positions
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // Set the AnimationCurve for width to have the same value at start, middle, and end
            AnimationCurve widthCurve = new AnimationCurve(
                new Keyframe(0.0f, uniformWidth), // Start width
                new Keyframe(0.5f, uniformWidth), // Middle width
                new Keyframe(1.0f, uniformWidth)  // End width
            );
            lineRenderer.widthCurve = widthCurve;

            // Adjust color based on weight
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(GetColorFromWeight(weight), 0.0f), new GradientColorKey(GetColorFromWeight(weight), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
            lineObj.SetActive(true);
        }


        private Color GetColorFromWeight(float weight)
        {
            // Implement your logic to return a color based on the weight
            // For example, from green (low weight) to red (high weight)
            return Color.Lerp(Color.green, Color.red, weight);
        }




        public void HideWaypoints(List<SerialisableWaypoint> waypoints)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                //hide the waypoint
                Debug.Log("Hiding Waypoint");
            }
        }

        public void HideEdges(List<SerialisableEdge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                //hide the edge
                Debug.Log("Hiding Edge");
            }
        }


        public void ToggleHeatMap() {
            if (isHeatMapVisible) {
                HideHeatMap();
            } else {
                DrawHeatMap();
            }
        }

        public void DrawHeatMap() 
        {
            //draw the white screen
            ShowWhiteScreen();
            Debug.Log("Drawing Heatmap");
            //draw all waypoints and edges
            DrawWaypoints(allWaypoints);
            DrawEdges(allEdges);
            Debug.LogError("edges drawn");
            //switch tracking variable
            isHeatMapVisible = true;
        }

        public void HideHeatMap() 
        {
            //unload all waypoints and edges
            //unload the white screen
            HideWhiteScreen();
            Debug.Log("Hiding Heatmap");


            //switch tracking variable
            isHeatMapVisible = false;
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




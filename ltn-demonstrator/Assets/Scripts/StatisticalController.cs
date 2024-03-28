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
        public static List<SerialisableWaypoint> centriods { get; private set; }
        public static List<SerialisableEdge> emissionEdges { get; private set; }
        public static List<SerialisableEdge> usageEdges { get; private set; }

        //text for stats in the statistical measures screen
        public TMP_Text statsText;
        //loads a whitescreen object
        public GameObject whiteScreenPanel;
        public GameObject waypointPrefab;
        public GameObject linePrefab; 

        //public GameObject edgePrefab;

        public float TERMINATION_CRITERIA = 5.0f;
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
                Time.timeScale = 8;
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
                    Debug.LogError($"length of alledges = {allEdges.Count}, length of allwaypoints = {allWaypoints.Count}");
                    //SerialisePathDataSave();
                    getAllSerialisedPaths();
                    Debug.Log("Serialised PathData");
                    //calculate edge weights
                    List<Cluster> clusters = ClusterWaypoints(allWaypoints, 15);
                    CreateCentroidsForClusters(clusters, allWaypoints);
                    centriods = ExtractCentriods(clusters);
                    CalcEdgeWeightsByUsage();
                    CalcEdgeWeightsByEmissions();
                    Debug.Log("Calculated Edge Weights");

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
                                $"Traveller Type: {pd.vType}";

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
                    //!Enum.IsDefined(typeof(VehicleType), allPathData[i].vType) ||
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
            string totalDistance = TotalDistance();
            string rateOfDeviation = RateOfDeviation();
            string sumOfPollution = SumOfPollution();
            string rateOfPollution = RateOfPollution();
            finalString = $"Number of travellers: {totalNoOfTravellers} \nTotal travel time: {totalTravelTime} seconds\nAverage traveller velocity: {averageTravVelo} km/h\nTotal distance travelled: {totalDistance} km\nRate of deviation from original path: {rateOfDeviation} \nAverage rate of pollution: {rateOfPollution} g/vehicle \nTotal pollution: {sumOfPollution} g ";
            //Set the TMP object to the stats we calc
            statsText.text = finalString;
            
        }

        // Method to add PathData to the list.
        public void AddPathData(PathData pathData)
        {
            allPathData.Add(pathData);
        }


        public void RecieveEndTime (int ID, List<Edge> path, VehicleProperties vType) {
            UpdateEndTimeAndPath(ID, path, vType);
            finishedPaths++;
            Debug.LogError($"finished Paths = {finishedPaths}, term = {TERMINATION_CRITERIA}, num of spawned Travellers = {TravellerManager.Instance.noOfTravellers}");
            if(tm.FinishedFor(TERMINATION_CRITERIA)){
                endSim=true;
            }
        }

        public void UpdateEndTimeAndPath(int id, List<Edge> path, VehicleProperties vType) {
            foreach (var pathData in allPathData)
            {
                if (pathData.ID == id)
                {
                    pathData.endTime = Time.time; 
                    pathData.path = path;
                    pathData.vType = vType;
                    if (path == null) {
                        Debug.LogError("path is null");
                    }
                }
            }
        }


        //get all edges in the simulation
        private List<Edge> GetAllEdges() {
            if (Graph.Instance != null) {
                return Graph.Instance.GetAllEdges();
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


        private VehicleType ConvertToVehicleType(ModeOfTransport mode)
        {
            switch (mode)
            {
                case ModeOfTransport.Car:
                    return VehicleType.PersonalCar;
                case ModeOfTransport.SUV:
                    return VehicleType.SUV;
                case ModeOfTransport.Van:
                    return VehicleType.Van;
                case ModeOfTransport.Taxi:
                    return VehicleType.Taxi;
                case ModeOfTransport.Pedestrian:
                case ModeOfTransport.Bicycle: // If you treat Bicycle as Pedestrian in this context
                    return VehicleType.Pedestrian;
                // Map other modes of transport if needed
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), $"Not supported mode: {mode}");
            }
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
           // Debug.LogError($"allEdges length: {allEdges.Count}, allWaypoints length: {allWaypoints.Count}");
        }


        public List<SerialisableEdge> CreateSerialisablePath(List<Edge> path)
        {
            List<SerialisableEdge> serialisablePath = new List<SerialisableEdge>();


            foreach (Edge edge in path)
            {

                foreach (SerialisableEdge serialisableEdge in allEdges)
                {
                   
                    if (serialisableEdge.ID == edge.ID)
                    {
                        serialisablePath.Add(serialisableEdge);
                    }
                }
            }
            return serialisablePath;
        }

        public void getAllSerialisedPaths() {
            foreach (var pathData in allPathData) {
                pathData.serialisablePath = CreateSerialisablePath(pathData.path);
            }
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

            Button heatmapToggleButtonU = GameObject.Find("Heatmap Toggle Usage").GetComponent<Button>();
            if (heatmapToggleButtonU != null) {
                heatmapToggleButtonU.onClick.RemoveAllListeners(); // Remove existing listeners to prevent stacking if scene is reloaded
                heatmapToggleButtonU.onClick.AddListener(ToggleHeatMapUsage);
                Debug.Log("Added listener to Heatmap Toggle Button - Usage.");
            } else {
                Debug.LogError("Failed to find the Heatmap Toggle Usage Button GameObject.");
            }
            //add second button here----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Button heatmapToggleButtonE = GameObject.Find("Heatmap Toggle Emission").GetComponent<Button>();
            if (heatmapToggleButtonE != null) {
                heatmapToggleButtonE.onClick.RemoveAllListeners(); // Remove existing listeners to prevent stacking if scene is reloaded
                heatmapToggleButtonE.onClick.AddListener(ToggleHeatMapEmission);
                Debug.Log("Added listener to Heatmap Toggle Button Emission.");
            } else {
                Debug.LogError("Failed to find the Heatmap Emission Toggle Button GameObject.");
            }
            //close heatmap button
            Button heatmapToggleButtonC = GameObject.Find("Heatmap Close").GetComponent<Button>();
            if (heatmapToggleButtonC != null) {
                heatmapToggleButtonC.onClick.RemoveAllListeners(); // Remove existing listeners to prevent stacking if scene is reloaded
                heatmapToggleButtonC.onClick.AddListener(ToggleHeatmapClose);
                Debug.Log("Added listener to Heatmap Close Toggle Button.");
            } else {
                Debug.LogError("Failed to find the Heatmap close Toggle Button GameObject.");
            }
        }
        


        public void CalculateTransform(List<SerialisableWaypoint> waypoints, float margin = 0.9f)
        {
            // get the dimensions of the panel
            GameObject panelGameObject = GameObject.Find("WhiteScreen");
            RectTransform panelRectTransform = panelGameObject.GetComponent<RectTransform>();
            Vector2 panelSize = panelRectTransform.rect.size;

            //Debug.LogError($" panelSize = {panelSize}");

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

            //Debug.Log($"minx = {minx}, maxx = {maxx}, minz = {minz}, maxz = {maxz}");

            float xStretch = (panelSize.x / (maxx - minx))*margin;
            float zStretch = (panelSize.y / (maxz - minz))*margin;

            //Debug.Log($"xStretch = {xStretch}, zStretch = {zStretch}");

            //
            foreach (var waypoint in waypoints)
            {
                waypoint.x = ((waypoint.x - minx) / (maxx - minx) * panelSize.x * margin) + (panelSize.x * (1 - margin) / 2);
                waypoint.z = ((waypoint.z - minz) / (maxz - minz) * panelSize.y * margin) + (panelSize.y * (1 - margin) / 2);

            }
        }



        public void DrawWaypoints(List<SerialisableWaypoint> waypoints, float scale = 1.0f)
        {
            //Transform waypoints so that they are correctly oriented
            CalculateTransform(waypoints);
            for (int i = 0; i < waypoints.Count; i++)
            {
                //draw the waypoint
                //Debug.Log($"Waypoint {waypoints[i].ID} drawn at {waypoints[i].x}, {waypoints[i].y}, {waypoints[i].z}");
                //create gameObject in unity for each waypoint and place it at the coordinates
                // Create the waypoint position from the serialized data
                Vector3 waypointPosition = new Vector3(waypoints[i].x, waypoints[i].z, -3); //(x,y,z)
                // Instantiate the waypoint prefab at this position
                waypoints[i].WaypointObject =  Instantiate(waypointPrefab, waypointPosition, Quaternion.identity);
                //Debug.Log("Spawned");
                // Alter the scale of the waypoint object
                Vector3 scalar = new Vector3(scale, scale, scale); // Set the desired scale here
                waypoints[i].WaypointObject.transform.localScale = scalar;


            }

        }


        public void DrawEdges(List<SerialisableEdge> edges, bool emissions)
        {
            Debug.LogError("Drawing Edges");
            for (int i = 0; i < edges.Count; i++)
            {
                //draw the edge
                //Debug.Log($"Edge {edges[i].ID} drawn from {edges[i].startWaypoint.ID} to {edges[i].endWaypoint.ID}");

                Vector3 startPoint = new Vector3(edges[i].startWaypoint.x, edges[i].startWaypoint.z, -1);
                Vector3 endPoint = new Vector3(edges[i].endWaypoint.x, edges[i].endWaypoint.z, -1);

                DrawLine(edges[i], startPoint, endPoint, emissions);   
                //Debug.Log($"Edge weight is {edges[i].weight}");
            }
        }



        public void DrawLine(SerialisableEdge edge, Vector3 start, Vector3 end, bool emissions)
        {   
            float uniformWidth;
            if (emissions == true) {
                // If you want a really wide line, you can set this directly to a large number
                uniformWidth = 1f + (30f*edge.weightEmissions); // for example, a very wide line
            }
            else {
                // If you want a really wide line, you can set this directly to a large number
                uniformWidth = 1f + (30f*edge.weightUsage); // for example, a very wide line
            }

            //translate the start and end points to the correct position, set z to -3
            start.z = -3;
            end.z = -3;

            GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();

            // Set the positions
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // Set the width of the line to be the same at the start, mi ddle, and end
            lineRenderer.startWidth = uniformWidth;
            lineRenderer.endWidth = uniformWidth;

            // Adjust color based on weight (you'll replace this part with your own method to get color from weight)
            // Get color from weight
            Color colorFromWeight;
            if (emissions == true) {
                colorFromWeight = GetColorFromWeight(edge.weightEmissions);
            }
            else {
                colorFromWeight = GetColorFromWeight(edge.weightUsage);
            }
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorFromWeight, 0.0f), new GradientColorKey(colorFromWeight, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;

            // Enable the line object
            lineObj.SetActive(true);

            // Assign the GameObject to the serializable edge
            edge.EdgeObject = lineObj;
        }



        private Color GetColorFromWeight(float weight)
        {
            if (weight > 0.4f)
                return Color.Lerp(Color.grey, new Color(220f / 255, 20f / 255, 60f / 255), 0.5f); // Subtle Crimson
            else if (weight > 0.3f)
                return Color.Lerp(Color.grey, new Color(1f, 0.4f, 0.4f), 0.5f); // Subtle Light Red
            else if (weight > 0.2f)
                return Color.Lerp(Color.grey, new Color(1f, 165f / 255, 0), 0.5f); // Subtle Orange
            else if (weight > 0.1f)
                return Color.Lerp(Color.grey, Color.yellow, 0.5f); // Subtle Yellow
            else
                return Color.Lerp(Color.grey, Color.green, 0.5f); // Subtle Green
        }






        public void HideWaypoints(List<SerialisableWaypoint> waypoints)
        {
            Debug.Log("Hiding Waypoints");
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] != null && waypoints[i].WaypointObject != null)
                {
                    //hide the waypoint
                waypoints[i].WaypointObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Waypoint or WaypointObject at index {i} is null");
                }
            }
        }

        public void HideEdges(List<SerialisableEdge> edges)
        {
            Debug.Log("Hiding Edges");
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] != null && edges[i].EdgeObject != null)
                {
                    //hide the edge
                    edges[i].EdgeObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Edge or EdgeObject at index {i} is null");
                }
            }
        }


        public void ToggleHeatMapUsage() {
            //hide all current edges
            HideEdges(allEdges);
            HideWaypoints(centriods);
            HideWaypoints(allWaypoints);
            DrawHeatMapUsage();
        }

        public void ToggleHeatMapEmission() {
            HideEdges(allEdges);
            HideWaypoints(centriods);
            HideWaypoints(allWaypoints);
            DrawHeatMapEmission();
        }

        public void ToggleHeatmapClose() {
            if (isHeatMapVisible == true) {
                HideHeatMap();
            }
        }


        public void DrawHeatMapUsage() 
        {
            //draw the white screen
            ShowWhiteScreen();
            Debug.Log("Drawing Heatmap");
            //draw all waypoints and edges
            DrawWaypoints(allWaypoints, 0.2f);        //make these smaller
            DrawWaypoints(centriods, 15f);     //make these bigger, make permanent attribute
            DrawEdges(allEdges, false);                //make two sets, one for pollution one for usage
            //switch tracking variable
            isHeatMapVisible = true;
        }

        public void DrawHeatMapEmission() 
        {
            //draw the white screen
            ShowWhiteScreen();
            Debug.Log("Drawing Heatmap");
            //draw all waypoints and edges
            DrawWaypoints(allWaypoints, 0.2f);        //make these smaller
            DrawWaypoints(centriods, 15f);     //make these bigger, make permanent attribute
            DrawEdges(allEdges, true);                //make two sets, one for pollution one for usage
            //switch tracking variable
            isHeatMapVisible = true;
        }


        public void HideHeatMap() 
        {
            //unload all waypoints and edges
            //unload the white screen
            HideWhiteScreen();
            Debug.Log("Hiding Heatmap");
            //hide all waypoints and edges
            HideWaypoints(allWaypoints);
            HideWaypoints(centriods);
            HideEdges(allEdges);

            //switch tracking variable
            isHeatMapVisible = false;
        }


        public void CalcEdgeWeightsByUsage () {
            //iterate through all pathdata serialisable paths
            float denominator = (float)allPathData.Count;
            foreach (PathData pd in allPathData) {
                foreach (SerialisableEdge e in pd.serialisablePath) {
                    //increment pathdata weight, normalise the amount
                    e.weightUsage += 1f / denominator;
                }
            }
        }

        public void CalcEdgeWeightsByEmissions () {
            //iterate through all pathdata serialisable paths
            float denominator = (float)allPathData.Count;
            foreach (PathData pd in allPathData) {
                float pathLength = 0;
                float currentWeight = 0;
                float currentEmission = pd.vType.RateOfEmission;
                foreach (SerialisableEdge e in pd.serialisablePath) {
                    //increment pathdata weight by emission score*path length, normalise the amount
                    float increment = (currentEmission/denominator)*10;
                    e.weightEmissions += increment;
                }
            }
        }


    public List<Cluster> ClusterWaypoints(List<SerialisableWaypoint> waypoints, double threshold) {
        List<Cluster> clusters = new List<Cluster>();
        foreach (var waypoint in waypoints) {
            bool assignedCluster = false;
            Vector3 waypointPosition = new Vector3(waypoint.x, waypoint.y, waypoint.z); // Convert to Vector3
            //Debug.Log($"Within waypoint loop, length of clusters = {clusters.Count}");
            foreach (var cluster in clusters) {
                foreach (var clusterWaypoint in cluster.Waypoints) {
                    Vector3 clusterWaypointPosition = new Vector3(clusterWaypoint.x, clusterWaypoint.y, clusterWaypoint.z); // Convert to Vector3
                    if (Vector3.Distance(waypointPosition, clusterWaypointPosition) < threshold) {
                        //Debug.Log("Adding waypoint to cluster");
                        cluster.Waypoints.Add(waypoint);
                        assignedCluster = true;
                        break;
                    }
                }
            }
            if (!assignedCluster) {
                //Debug.Log("Creating new cluster");
                Cluster newCluster = new Cluster();
                newCluster.AddWaypoint(waypoint);
                clusters.Add(newCluster);
            }
        }
        return clusters;
    }

    public void CreateCentroidsForClusters(List<Cluster> clusters, List<SerialisableWaypoint> waypoints) {
        int nextID = waypoints.Count + 1;
        foreach (var cluster in clusters) {
            float x = 0;
            float y = 0;
            float z = 0;
            foreach (var waypoint in cluster.Waypoints) {
                x += waypoint.x;
                y += waypoint.y;
                z += waypoint.z;
            }
            x /= cluster.Waypoints.Count;
            y /= cluster.Waypoints.Count;
            z /= cluster.Waypoints.Count;
            cluster.centroid = new SerialisableWaypoint(nextID++, x,y,z);
            //Debug.Log($"Centroid for cluster: {cluster.centroid.x}, {cluster.centroid.y}, {cluster.centroid.z}");
        }
    }

    public List<SerialisableWaypoint> ExtractCentriods (List<Cluster> clusters) {
        List<SerialisableWaypoint> centroids = new List<SerialisableWaypoint>();
        foreach (var cluster in clusters) {
            centroids.Add(cluster.centroid);
        }
        return centroids;
    }


    public List<SerialisableEdge> MakeEdgesForCentriods(List<SerialisableEdge> edges, List<Cluster> clusters) 
    {
        List<SerialisableEdge> newEdges = new List<SerialisableEdge>();
        int edgeID = edges.Count + 1;
        // for each edge figure out which cluster it belongs to
        foreach (var edge in edges) {
            SerialisableWaypoint start = edge.startWaypoint;
            SerialisableWaypoint end = edge.endWaypoint;
            Cluster startCluster = null;
            Cluster endCluster = null;
            foreach (var cluster in clusters) {
                foreach (var waypoint in cluster.Waypoints) {
                    if (waypoint == start) {
                        startCluster = cluster;
                    }
                    if (waypoint == end) {
                        endCluster = cluster;
                    }
                }
            }
            // if both clusters are not null, create a new edge between the two centroids
            if (startCluster != null && endCluster != null) {

                //check that the edge doesnt already exist
                bool edgeExists = false;
                SerialisableEdge edgeN = null;
                foreach (var edgeC in startCluster.edges) {
                    if ((edge.startWaypoint == startCluster.centroid && edge.endWaypoint == endCluster.centroid) || 
                        (edge.startWaypoint == endCluster.centroid && edge.endWaypoint == startCluster.centroid)) 
                    {
                        edgeExists = true;
                        edgeN = edgeC;
                        break;
                    }
                }
                if (!edgeExists) {
                    //Debug.Log("Creating new edge");
                    // calculate the length of the edge
                    float length = Vector3.Distance(new Vector3(start.x, start.y, start.z), new Vector3(end.x, end.y, end.z));
                    // create a new edge between the two centroids
                    SerialisableEdge newEdge = new SerialisableEdge(edgeID++, startCluster.centroid, endCluster.centroid, length, edge.weightEmissions);
                    // add the new edge to the list of edges
                    newEdges.Add(newEdge);
                }
                else {
                    // add their weight to the existing edge
                    edgeN.IncrementEdgeWeight(edge.weightEmissions);
                }
            }
        }
        return newEdges;
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
            totalTravelTime = (float)(totalTravelTime);
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
                    //Debug.Log(Time.deltaTime);
                    continue; // Skip this PathData as its path is null
                }
                foreach (SerialisableEdge e in pd.serialisablePath)
                {
                    if (e == null)
                    {
                        //Debug.LogWarning("Edge in path is null");
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
            //Needs delta D conversion, time conversion (Time.deltaTime)
            float averageVelocity = (totalDistance) / (totalTime * numberOfTravellers);
            float normalisedAverageVelocity = averageVelocity * 3.6f; //convert to km/h
            return normalisedAverageVelocity.ToString();
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



        //Sum of pollution - uses arbitrary values for now
        private string SumOfPollution()
        {
            float totalPollution = 0;
            if (allPathData == null) 
            {
                Debug.LogError("allPathData is null");
                return "N/A";
            }

            foreach (PathData pd in allPathData)
            {
                // Create an instance of VehicleProperties with the traveller type

                //
                float pathLength = 0;

                foreach (SerialisableEdge e in pd.serialisablePath)
                {
                    pathLength += e.length;
                }

                totalPollution += pd.vType.RateOfEmission * pathLength;
            }

            return totalPollution.ToString();
        }




        //rate of Pollution - sum / no. of travellers
        private string RateOfPollution()
        {
            float sum = 0;
            float.TryParse(SumOfPollution(), out sum);
            int noOfTravellers = 0;
            int.TryParse(TotalNumberOfTravellers(), out noOfTravellers);
            if (noOfTravellers == 0)
            {
                return "0"; // Avoid division by zero
            }
            return (sum / noOfTravellers).ToString();
        }


        private string TotalDistance () {
            float totalDistance = 0;
            foreach (var pathData in allPathData) {
                Debug.LogError($"pathData.serialisablePath length = {pathData.serialisablePath.Count}");
                foreach (var edge in pathData.serialisablePath) {
                    totalDistance += edge.length;
                }
            }
            return (totalDistance/1000).ToString();
        }




//------------------------------------------PHASE TWO PARETO FRONT-------------------------------------------


    public void SaveObjectiveValuesToJson () 
    {
        //TODO
    }


    public List<string> GetFileNameOfNextJson ()
    {
        //TODO
        return new List<string>();
    }










}




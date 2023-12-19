using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    Waypoint waypoint;
    GameObject graphGameObject;

    // Adding fields to store the state of the toggles
    private bool isSingleConnection = false;
    private bool isBidirectional = true;

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }
        waypoint = (Waypoint) target;
        graphGameObject = (GameObject) waypoint.transform.parent.gameObject;
    }

    /// <summary>
    /// This runs whenever a waypoint is deleted, and prunes any null adjacent waypoints,
    /// as well as updating the edges.
    /// </summary>
    private void OnDisable()
    {
        // as we've changed the waypoints, we need to reload the edges
        PruneDeletedWaypoints();
        EdgeLoader.LoadEdges();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Selected Waypoint: " + waypoint.name);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Connection Type:");

        EditorGUI.BeginChangeCheck();

        isSingleConnection = GUILayout.Toggle(isSingleConnection, "Single", "Button");
        if (GUI.changed)
        {
            isBidirectional = false;
            GUI.changed = false; // Reset the GUI.changed state
        }

        isBidirectional = GUILayout.Toggle(isBidirectional, "Bidirectional", "Button");
        if (GUI.changed)
        {
            isSingleConnection = false;
        }

        EditorGUI.EndChangeCheck();

        GUILayout.EndHorizontal();

        // Enable the button only if one of the toggles is true
        GUI.enabled = isSingleConnection || isBidirectional;

        if (GUILayout.Button("Create Adjacent Waypoint"))
        {
            CreateAdjacentWaypoint(isSingleConnection);
        }

        // Restore GUI.enabled state to true for any other GUI elements that may follow
        GUI.enabled = true;
    }

    /// <summary>
    /// This method is called when the user clicks to create a new waypoint. It also adds the new waypoint to the
    /// selected waypoint's adjacent waypoints. This works for Bidirectional connections atm
    /// </summary>
    void CreateAdjacentWaypoint()
    {
        GameObject newWaypointPrefab = Resources.Load<GameObject>("Waypoint");
        GameObject newWaypoint = Instantiate(newWaypointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        newWaypoint.transform.position = waypoint.transform.position; // Adjust as needed
        newWaypoint.name = "Waypoint (" + graphGameObject.transform.childCount + ")";

        waypoint.AddAdjacentWaypoint(newWaypoint.GetComponent<Waypoint>());
        // this makes sure the edge loader is up to date
        EdgeLoader.LoadEdges();

        // Add the new waypoint to the graph
        newWaypoint.transform.parent = graphGameObject.transform;

        // Make the new waypoint the selected waypoint
        Selection.activeGameObject = newWaypoint;
    }

    /// <summary>
    /// This method is called when a waypoint is deleted. It removes any null adjacent waypoints.
    /// This is necessary because when a waypoint is deleted, the adjacent waypoints are not updated.
    /// This can be called manually from the editor, or automatically when a waypoint is deleted.
    /// </summary>
    [MenuItem("Tools/Prune Deleted Waypoints")] [InitializeOnLoadMethod]
    static void PruneDeletedWaypoints()
    {
        Waypoint[] allWaypoints = FindObjectsOfType<Waypoint>();

        // we go through all the waypoints, and if one of them has a null adjacent waypoint, we remove it
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            for (int j = 0; j < allWaypoints[i].adjacentWaypoints.Count; j++)
            {
                if (allWaypoints[i].adjacentWaypoints[j] == null)
                {
                    allWaypoints[i].adjacentWaypoints.RemoveAt(j);
                }
            }
        }

        // Mark all modified objects as dirty so the changes are saved
        foreach (Waypoint waypoint in allWaypoints)
        {
            EditorUtility.SetDirty(waypoint);
        }
    }
    
}

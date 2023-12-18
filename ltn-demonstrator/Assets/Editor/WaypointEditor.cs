using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    Waypoint waypoint;
    GameObject graphGameObject;
    const float selectionRadius = 2f; // Adjust the radius as needed for your scene scale

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

    void CreateAdjacentWaypoint(bool isSingleConnection)
    {
        GameObject newWaypointPrefab = Resources.Load<GameObject>("Waypoint");
        GameObject newWaypoint = Instantiate(newWaypointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        newWaypoint.transform.position = waypoint.transform.position; // Adjust as needed
        newWaypoint.name = "Waypoint (" + graphGameObject.transform.childCount + ")";

        // Get the Waypoint component from the new waypoint
        Waypoint newWaypointComponent = newWaypoint.GetComponent<Waypoint>();

        // Add the adjacent waypoint with specified edge type
        waypoint.AddAdjacentWaypoint(newWaypointComponent, isSingleConnection);

        // Update the edge loader
        EdgeLoader.LoadEdges();

        // Add the new waypoint to the graph
        newWaypoint.transform.parent = graphGameObject.transform;

        // Make the new waypoint the selected waypoint
        Selection.activeGameObject = newWaypoint;
    }
}

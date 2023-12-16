using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    Waypoint waypoint;
    GameObject graphGameObject;
    const float selectionRadius = 2f; // Adjust the radius as needed for your scene scale

    private void OnEnable()
    {
        waypoint = (Waypoint) target;
        graphGameObject = (GameObject) waypoint.transform.parent.gameObject;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Selected Waypoint: " + waypoint.name);

        if (GUILayout.Button("Create Adjacent Waypoint"))
        {
            CreateAdjacentWaypoint();
        }
    }

    void CreateAdjacentWaypoint()
    {
        GameObject newWaypointPrefab = Resources.Load<GameObject>("Waypoint");
        GameObject newWaypoint = Instantiate(newWaypointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        newWaypoint.transform.position = waypoint.transform.position; // Adjust as needed

        // these getcomponents are messy and should be refactored
        waypoint.AddAdjacentWaypoint(newWaypoint.GetComponent<Waypoint>());

        // add the new waypoint to the graph
        newWaypoint.transform.parent = graphGameObject.transform; 

        // make the new waypoint the selected waypoint
        Selection.activeGameObject = newWaypoint;
    }
}

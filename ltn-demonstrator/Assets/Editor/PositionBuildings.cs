using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PositionBuildings : EditorWindow
{
    private static float buildingOffset = 10f; // Set this to your desired offset distance

    [MenuItem("Tools/Buildings/Rotate Buildings to Face Roads")]
    public static void RotateBuildings()
    {
        List<Building> buildings = new List<Building>(Object.FindObjectsOfType<Building>());

        foreach (Building building in buildings)
        {
            Vector3 closestPoint = building.CalcClosestPointOnEdge(); // Assuming this returns the closest point on the edge
            Vector3 directionToClosestPoint = closestPoint - building.transform.position; // Calculate direction vector

            // Ensure the direction vector is not zero (which happens when the positions are the same)
            if (directionToClosestPoint != Vector3.zero)
            {
                building.transform.rotation = Quaternion.LookRotation(directionToClosestPoint.normalized, Vector3.up);
            }
        }

        // mark scene as dirty so it saves
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Buildings/Position Buildings on Roads")]
    public static void ShowWindow()
    {
        GetWindow<PositionBuildings>("Position Buildings");
    }

    void OnGUI()
    {
        GUILayout.Label("Configure Building Position", EditorStyles.boldLabel);
        buildingOffset = EditorGUILayout.FloatField("Offset Distance", buildingOffset);

        if (GUILayout.Button("Position Buildings"))
        {
            PositionBuildingsOnRoads(buildingOffset);
        }
    }

    public static void PositionBuildingsOnRoads(float offset)
    {
        List<Building> buildings = new List<Building>(Object.FindObjectsOfType<Building>());

        foreach (Building building in buildings)
        {
            Vector3 closestPoint = building.CalcClosestPointOnEdge(); // Assuming this returns the closest point on the edge
            Vector3 directionFromClosestPointToBuilding = building.transform.position - closestPoint; // Calculate direction vector

            if (directionFromClosestPointToBuilding != Vector3.zero)
            {
                // Normalize the direction vector
                Vector3 normalizedDirection = directionFromClosestPointToBuilding.normalized;

                // Calculate the new position by moving "offsetDistance" units towards the building from the closest point
                Vector3 newPosition = closestPoint + normalizedDirection * buildingOffset;

                // Set the building's position to this new position
                building.transform.position = newPosition;
            }
        }

        // mark scene as dirty so it saves
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}

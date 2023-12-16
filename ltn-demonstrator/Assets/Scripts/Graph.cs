using UnityEngine;

public class Graph : MonoBehaviour
{
    // private waypointsize with getter
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 0.5f;

    public float WaypointSize
    {
        get { return waypointSize; }
    }

    public float CalculateDistance(Waypoint a, Waypoint b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public float CalculateDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
}

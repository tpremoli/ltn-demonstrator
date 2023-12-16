using UnityEngine;

public class Graph : MonoBehaviour
{
    [Range(0f, 2f)]

    // private waypointsize with getter
    [SerializeField] private float waypointSize = 0.5f;

    public float WaypointSize
    {
        get { return waypointSize; }
    }

}


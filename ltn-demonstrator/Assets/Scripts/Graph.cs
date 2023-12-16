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

}


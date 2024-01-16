using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(WaypointMover))]
class LabelHandle : Editor
{
    // this editor is used to display the current velocity and leftToMove
    // of the WaypointMover in the scene view, for debugging purposes
    void OnSceneGUI()
    {
        if (!(target is WaypointMover))
        {
            return;
        }
        WaypointMover wp = (WaypointMover)target;
        if (wp == null)
        {
            return;
        }

        Handles.color = Color.blue;
        Handles.Label(wp.transform.position + Vector3.up * 3,
            " ltm: " + wp.leftToMove + "\n vel: " + wp.velocity);
    }
}
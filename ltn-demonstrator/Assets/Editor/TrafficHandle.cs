using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(WaypointMover))]
class LabelHandle : Editor
{
    void OnSceneGUI()
    {
        if (!(target is WaypointMover)){
            return;
        }
        WaypointMover wp = (WaypointMover)target;
        if (wp == null)
        {
            return;
        }

        Handles.color = Color.blue;
        Handles.Label(wp.transform.position + Vector3.up * 3,
            " ltm: "+wp.leftToMove+"\n vel: "+wp.velocity);
    }
}
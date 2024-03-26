using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor;
//using UnityEditor.SceneManagement;


/*
UniqueID is a scripting component that can be attached to any game object/prefab
to give that particular game object/prefab instance a unique identifier. This ID
is persistent, so if Unity is closed and opened again, the ID will persist and
remain the same.
*/
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    static Dictionary<string, UniqueID> allGUIDs = new Dictionary<string, UniqueID>();
    public string uniqueID;

    void Update()
    {
        if (Application.isPlaying || uniqueID != "") {
            return;
        }

        GenerateUniqueID();
    }

    void GenerateUniqueID() {
        bool anotherComponentAlreadyHasGUID = (
            uniqueID != null &&
            allGUIDs.ContainsKey(uniqueID) &&
            allGUIDs[uniqueID] != this
        );
        
        if (uniqueID == "" || anotherComponentAlreadyHasGUID) {
            uniqueID = Guid.NewGuid().ToString();
  //          EditorUtility.SetDirty(this);
  //          EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        if (!allGUIDs.ContainsKey(uniqueID)) {
            allGUIDs.Add(uniqueID, this);
        }
    }

    void OnDestroy() {
        allGUIDs.Remove(uniqueID);
    }
}
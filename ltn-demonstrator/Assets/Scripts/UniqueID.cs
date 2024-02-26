using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    static Dictionary<string, UniqueID> allGUIDs = new Dictionary<string, UniqueID>();
    public string uniqueID;

    #if UNITY_EDITOR
    void Update()
    {
        if (Application.isPlaying) {
            return;
        }

        bool anotherComponentAlreadyHasGUID = (
            uniqueID != null &&
            allGUIDs.ContainsKey(uniqueID) &&
            allGUIDs[uniqueID] != this
        );
        
        if (uniqueID == "" || anotherComponentAlreadyHasGUID) {
            uniqueID = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        if (!allGUIDs.ContainsKey(uniqueID)) {
            allGUIDs.Add(uniqueID, this);
        }
    }

    void OnDestroy() {
        allGUIDs.Remove(uniqueID);
    }
    #endif
}

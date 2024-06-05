using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
[System.Serializable]
public class UniqueID : MonoBehaviour
{
    static Dictionary<string, UniqueID> allGUIDs = new Dictionary<string, UniqueID>();
    [SerializeField] public string uniqueID;

    void Update()
    {
        if (Application.isPlaying || !string.IsNullOrEmpty(uniqueID)) {
            return;
        }

        GenerateUniqueID();
    }

    public void GenerateUniqueID() {
        bool anotherComponentAlreadyHasGUID = (
            uniqueID != null &&
            allGUIDs.ContainsKey(uniqueID) &&
            allGUIDs[uniqueID] != this
        );
        
        if (string.IsNullOrEmpty(uniqueID) || anotherComponentAlreadyHasGUID) {
            uniqueID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        if (!allGUIDs.ContainsKey(uniqueID)) {
            allGUIDs.Add(uniqueID, this);
        }
    }

    void OnValidate() {
        if (string.IsNullOrEmpty(uniqueID)) {
            GenerateUniqueID();
        }
    }

    void OnDestroy() {
        allGUIDs.Remove(uniqueID);
    }
}

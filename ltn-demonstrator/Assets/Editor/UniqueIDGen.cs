using UnityEngine;
using UnityEditor;

public class UniqueIDEditorWindow : EditorWindow
{
    [MenuItem("Tools/Unique ID Manager")]
    public static void ShowWindow()
    {
        GetWindow<UniqueIDEditorWindow>("Unique ID Manager");
    }

    void OnGUI()
    {
        GUILayout.Label("Unique ID Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("Regenerate Unique IDs"))
        {
            RegenerateUniqueIDs();
        }
    }

    void RegenerateUniqueIDs()
    {
        UniqueID[] uniqueIDComponents = FindObjectsOfType<UniqueID>();
        foreach (var uniqueIDComponent in uniqueIDComponents)
        {
            uniqueIDComponent.GenerateUniqueID();
        }

        Debug.Log("Regenerated Unique IDs for all UniqueID components.");
    }
}

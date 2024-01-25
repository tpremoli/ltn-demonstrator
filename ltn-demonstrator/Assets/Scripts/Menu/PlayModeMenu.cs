using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayModeMenu : MonoBehaviour
{
    public void OnEditLTNButtonPressed()
    {
        // Load the EditLTN scene
        Debug.LogError("EditLTNButton.cs: OnButtonPressed(");
        SceneManager.LoadScene("EditLTN");
    }
}


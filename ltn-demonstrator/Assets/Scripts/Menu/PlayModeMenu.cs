using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayModeMenu : MonoBehaviour
{
    public void OnEditLTNButtonPressed()
    {
        // Load the EditLTN scene
        Debug.Log("EditLTNButton.cs: OnButtonPressed(");
        UnityEngine.SceneManagement.SceneManager.LoadScene("EditLTN");
    }
}


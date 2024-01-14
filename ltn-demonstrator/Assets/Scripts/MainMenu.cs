using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the next scene in the build order
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        // Load a specific scene by name
        SceneManager.LoadScene("Drag and Drop");
    }

    public void EditLTN()
    {
        SceneManager.LoadScene("Drag and Drop");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayButton : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        // Load the MenuProperMapScene scene
        SceneManager.LoadScene("MenuProperMapScene");
    }
}

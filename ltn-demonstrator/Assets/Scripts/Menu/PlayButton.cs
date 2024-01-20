using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PlayButton : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        // Load the ProperMapScene scene
        SceneManager.LoadScene("ProperMapScene");
    }
}

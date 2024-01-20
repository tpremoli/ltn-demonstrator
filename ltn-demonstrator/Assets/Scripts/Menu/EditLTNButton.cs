using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EditLTNButton : MonoBehaviour
{
    public void OnButtonPressed()
    {
        // Load the ProperMapScene scene
        SceneManager.LoadScene("BarrierButtonScene");
    }
}


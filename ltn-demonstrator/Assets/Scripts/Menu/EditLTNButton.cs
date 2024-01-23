using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EditLTNButton : MonoBehaviour
{
    public void OnButtonPressed()
    {
        // Load the EditLTN scene
        SceneManager.LoadScene("EditLTN");
    }
}


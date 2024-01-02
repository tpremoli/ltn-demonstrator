using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenu;
    void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;  
    }

    void Play()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

}

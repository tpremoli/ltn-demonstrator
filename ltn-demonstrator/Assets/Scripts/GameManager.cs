using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isPaused = true;

    // Start is called before the first frame update
    void Start()
{
    isPaused = true;
    Time.timeScale = 0f; // Game starts paused
}


    // Update is called once per frame
    void Update()
    {
        // Check for user input to unpause the game
        if (isPaused && Input.GetKeyDown(KeyCode.Space))
        {
            UnpauseGame();
        }
    }

    // Implement a method to pause the game
    void PauseGame()
    {
        Time.timeScale = 0f; // Set time scale to 0 to pause the game
        isPaused = true;
    }

    // Implement a method to unpause the game
    void UnpauseGame()
    {
        Time.timeScale = 1f; // Set time scale to 1 to resume the game
        isPaused = false;
    }

    // Implement any other game management methods as needed
}

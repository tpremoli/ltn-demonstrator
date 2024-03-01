using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }


    private void Awake()
    {
        Debug.Log("working");
        //create instance of self if non-existant
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make the TravellerManager's GameObject persistent
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance is active.
        }

        //create instance of TravellerManager if non-existant
        if (TravellerManager.Instance == null)
        {
            GameObject tmGameObject = new GameObject("TravellerManager");
            tmGameObject.AddComponent<TravellerManager>();  // This will set the Instance if the singleton pattern is implemented correctly
            DontDestroyOnLoad(tmGameObject);
        }
        //create instance of statistics manager if non-existant
        if (StatisticsManager.Instance == null)
        {
            GameObject smGameObject = new GameObject("StatisticsManager");
            smGameObject.AddComponent<StatisticsManager>();  // This will set the Instance if the singleton pattern is implemented correctly
            DontDestroyOnLoad(smGameObject);
        }
    }



    // Rest of your manager code...
    // spawner of events
}

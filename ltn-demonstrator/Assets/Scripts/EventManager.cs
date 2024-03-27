using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventManager : MonoBehaviour
{
    List<PersistentTraveller> travellers = new List<PersistentTraveller>();
    public List<Journey> eventList = new List<Journey>();
    public int numberOfTravellers;

    // Maximum number of journeys to complete.
    public int maxJourneys;

    // Likelihood that the traveller is from the local area.
    public float localTravellerProbability;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Graph.Instance.IsInitialised);

        numberOfTravellers = 50;
        maxJourneys = 8;
        localTravellerProbability = 0.7f;

        for (int i = 0; i < numberOfTravellers; i++) {
            // Create a new persistent traveller and generate some journeys for it to complete.
            Debug.Log("Creating new traveller...");
            PersistentTraveller t = new PersistentTraveller();
            t.Setup(GenerateJourneys(t));

            // Add the traveller to the list of travellers.
            travellers.Add(t);
        }
    }

    // Generate some random journeys for a persistent traveller.
    public List<Journey> GenerateJourneys(PersistentTraveller traveller) {
        Condition condition;
        Journey journey;
        List<Journey> journeys = new List<Journey>();

        // Choose a random number of journeys.
        int numberOfJourneys = Random.Range(0, maxJourneys);

        // Start journey always originates from either residence or through traffic node.
        // Randomly select building type based on probability that traveller is from local area.
        BuildingType chosenType = (Random.value < localTravellerProbability) ? BuildingType.Residence : BuildingType.ThroughTrafficDummy;

        // Choose some random time, random origin building, random destination building (ensure they are not the same).
        float time = Random.Range(1f, 3f);
        Building building = Graph.Instance.getRandomBuildingByType(chosenType);
        string originID = building.GetComponent<UniqueID>().uniqueID;
        string destinationID = Graph.Instance.pickRandomBuildingID();
        while (originID.Equals(destinationID)) {
            destinationID = Graph.Instance.pickRandomBuildingID();
        }

        // Construct new condition and journey.
        condition = new Condition(originID, time, traveller);
        journey = new Journey(originID, destinationID, time, traveller, condition);

        // Add to list of journeys.
        journeys.Add(journey);
        Debug.Log("Added initial journey to persistent traveller: " + journey);

        // Generate list of random times.
        List<float> times = new List<float>();
        for (int i = 0; i < numberOfJourneys; i++) {
            times.Add(Random.Range(3f, 27f));
        }
        // Sort list of times.
        times.Sort();

        for (int i = 0; i < numberOfJourneys; i++) {
            // Origin is destination of previous journey.
            originID = journeys[journeys.Count - 1].destination;

            // Select destination building.
            chosenType = BuildingProperties.getRandomWeightedDestinationType();
            building = Graph.Instance.getRandomBuildingByType(chosenType);
            destinationID = building.GetComponent<UniqueID>().uniqueID;

            // If destination is same as origin, choose a new building.
            while (destinationID.Equals(originID)) {
                // If the selected building type has less than 2 actual buildings in the simulation,
                // choose a new building type.
                if (Graph.Instance.buildingsByType[chosenType].Count <= 1) {
                    chosenType = BuildingProperties.getRandomWeightedDestinationType();
                }
                // Select a new building and get it's ID.
                building = Graph.Instance.getRandomBuildingByType(chosenType);
                destinationID = building.GetComponent<UniqueID>().uniqueID;
            }

            time = times[i];

            condition = new Condition(originID, time, traveller);
            journey = new Journey(originID, destinationID, time, traveller, condition);

            journeys.Add(journey);
            Debug.Log("Added additional journey to persistent traveller: " + journey);
        }


        // End journey is always from the destination of the last journey in the list
        // of journeys back to the origin of the first journey in the list of journeys.
        originID = journeys[journeys.Count - 1].destination;
        destinationID = journeys[0].origin;
        time = Random.Range(27f, 30f);

        condition = new Condition(originID, time, traveller);
        journey = new Journey(originID, destinationID, time, traveller, condition);

        journeys.Add(journey);
        Debug.Log("Added final journey to persistent traveller: " + journey);

        return journeys;
    }

    public void SaveEventListToJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            Debug.Log("There is already an event list JSON file: " + filePath);
        }
        else
        {
            string jsonData = JsonUtility.ToJson(eventList);
            File.WriteAllText(filePath, jsonData);
            Debug.Log("Event list saved to JSON: " + filePath);
        }
    }

    public void LoadEventListFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            eventList = JsonUtility.FromJson<List<Journey>>(jsonData);
            Debug.Log("Event list loaded from JSON: " + filePath);
            
        }
        else
        {
            Debug.LogWarning("Failed to load event list. JSON file not found: " + filePath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PersistentTraveller t in travellers) {
            Journey j = t.pickJourney();
            if (j != null) {
                eventList.Add(j);
                Debug.Log("Added journey to event list: " + j.origin + " to " + j.destination);
            }
        }
    }
}

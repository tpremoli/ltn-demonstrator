using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int numberOfJourneys = Random.Range(1, maxJourneys);

        // Start journey always originates from either residence or through traffic node.
        // Randomly select building type based on probability that traveller is from local area.
        BuildingType chosenType = (Random.value < localTravellerProbability) ? BuildingType.Residence : BuildingType.ThroughTrafficDummy;

        // Choose some random time, random origin building, random destination building (ensure they are not the same).
        float time = Random.Range(1f, 3f);
        Building origin = Graph.Instance.getRandomBuildingByType(chosenType);
        string originID = origin.GetComponent<UniqueID>().uniqueID;
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
            destinationID = Graph.Instance.pickRandomBuildingID();
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

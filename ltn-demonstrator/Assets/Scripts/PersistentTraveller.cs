using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentTraveller {
    List<Journey> journeys = new List<Journey>();

    public bool inTransit;
    public string currentLocation;
    public Journey currentJourney;
    public int journeyIndex;

    public PersistentTraveller() {
        //GenerateRandomJourneys();

        // Set in transit flag, current location and journey index.
        inTransit = false;
        journeyIndex = 0;
    }

    public void Setup(List<Journey> journeys) {
        // Set list of journeys.
        this.journeys = journeys;

        // Set current location based on list of journeys.
        currentLocation = this.journeys[journeyIndex].origin;
    }

    // Generate some random journeys to complete.
    public void GenerateRandomJourneys() {
        // Generate some random journeys to complete.
        for (int i = 0; i < 5; i++) {
            // Choose random origin and destination.
            string origin = Graph.Instance.pickRandomBuildingID();
            string destination = Graph.Instance.pickRandomBuildingID();
            while (origin.Equals(destination)) {
                // Pick random destination not equal to origin.
                destination = Graph.Instance.pickRandomBuildingID();
            }

            float time = Random.Range(0f, 50f);

            Condition condition = new Condition(origin, time, this);

            // Create new journey object and add to list of journeys.
            Journey j = new Journey(origin, destination, time, this, condition);
            journeys.Add(j);
        }

        // Order journeys by time.
        journeys = journeys.OrderBy(j => j.time).ToList();
    }

    // Add a journey to the list of journeys for this traveller.
    public void addJourney(Journey journey) {
        this.journeys.Add(journey);
    }

    public void journeyStarted(Journey journey) {
        // Set transit flag, clear current location, set current journey and set journey status to in progress.
        inTransit = true;
        currentLocation = "";
        currentJourney = journey;
        journey.status = JourneyStatus.InProgress;

        Debug.Log("Journey started: " + journey);
    }

    public void journeyCompleted(Journey journey) {
        // Unset transit flag, set current location, clear current journey and set journey status to completed.
        inTransit = false;
        currentLocation = journey.destination;
        currentJourney = null;
        journey.status = JourneyStatus.Completed;

        // Increment current journey index.
        journeyIndex += 1;

        Debug.Log("Journey completed: " + journey);
    }

    public void journeyAbandoned(Journey journey) {
        // Unset transit flag, reset current location, clear current journey and set journey status to abandoned.
        inTransit = false;
        currentLocation = journey.origin;
        currentJourney = null;
        journey.status = JourneyStatus.Abandoned;

        // Remove future journeys.
        removeFutureJourneys(journey);

        Debug.Log("Journey abandoned: " + journey);
    }

    public List<Journey> peekJourneys() {
        return journeys;
    }

    public Journey pickJourney() {
        // Check if all journeys completed: if so, exit.
        if (journeyIndex == journeys.Count) {
            return null;
        }
        
        Journey j = journeys[journeyIndex];
        Condition c = j.condition;

        if (c.ConditionMet(Time.time) && j.status == JourneyStatus.NotStarted) {
            return j;
        } else {
            return null;
        }
    }

    public void removeFutureJourneys(Journey journey) {

        for (int i = journeyIndex; i < journeys.Count; i++) {
            Journey j = journeys[i];

            if (journey.origin.Equals(j.origin)) {
                break;
            }
            else {
                j.status = JourneyStatus.Abandoned;
                journeyIndex += 1;

            }
        }
    }
}
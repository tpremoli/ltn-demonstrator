using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentTraveller {
    List<Journey> journeys = new List<Journey>();

    bool inTransit;
    string currentLocation;
    Journey currentJourney;

    public PersistentTraveller() {
        GenerateJourneys();

        // Set in transit flag and current location.
        inTransit = false;
        currentLocation = journeys[0].origin;
    }

    // Generate some random journeys to complete.
    public void GenerateJourneys() {
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

            // Create new journey object and add to list of journeys.
            Journey j = new Journey(origin, destination, time, this);
            journeys.Add(j);
        }

        // Order journeys by time.
        journeys = journeys.OrderBy(j => j.time).ToList();
    }

    public void journeyStarted(Journey journey) {
        inTransit = true;
        currentLocation = "";
        currentJourney = journey;
        journey.status = JourneyStatus.InProgress;
    }

    public void journeyCompleted(Journey journey) {
        currentJourney = null;
        currentLocation = journey.destination;
        inTransit = false;
    }

    public void setInTransit() {
        this.inTransit = true;
        this.currentLocation = null;
    }

    public void arrivedToDestination(string destination) {
        this.currentLocation = destination;
    }

    public List<Journey> peekJourneys() {
        return journeys;
    }

    public Journey pickJourney() {
        Journey j = journeys[0];

        if (currentLocation.Equals(j.origin) && Time.time > j.time && j.status == JourneyStatus.NotStarted) {
            return j;
        } else {
            return null;
        }
    }

    public void removeFutureJourneys(Journey journey) {

        for (int i = 0; i < journeys.Count; i++) {
            Journey j = journeys[i];

            if (journey.origin.Equals(j.origin)) {
                break;
            }
        }
    }
}
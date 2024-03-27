// Journey is a class used for describing a journey a persistent traveller should complete.
// It specifies the origin and destination, along with the time to start the journey and
// the traveller that needs to complete the journey.
public class Journey
{
    public string origin;
    public string destination;
    public float time;
    public PersistentTraveller traveller;
    public JourneyStatus status;
    public Condition condition;

    // Construct a new Journey object.
    public Journey(string origin, string destination, float time, PersistentTraveller traveller, Condition condition) {
        this.origin = origin;
        this.destination = destination;
        this.time = time;
        this.traveller = traveller;
        this.status = JourneyStatus.NotStarted;
        this.condition = condition;
    }

    public override string ToString() {
        return this.origin + " to " + this.destination + " at " + this.time;
    }
}

public enum JourneyStatus {
    NotStarted,
    InProgress,
    Completed,
    Abandoned,
}
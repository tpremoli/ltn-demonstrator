    // Journey is a class used for describing a journey a persistent traveller should complete.
// It specifies the origin and destination, along with the time to complete the journey and
// the traveller that needs to complete the journey.
public class Journey
{
    public string origin {get; private set;}
    public string destination {get; private set;}
    public float time {get; private set;}
    public PersistentTraveller traveller {get; private set;}
    public JourneyStatus status;

    // Construct a new Journey object.
    public Journey(string origin, string destination, float time, PersistentTraveller traveller) {
        this.origin = origin;
        this.destination = destination;
        this.time = time;
        this.traveller = traveller;
        this.status = JourneyStatus.NotStarted;
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
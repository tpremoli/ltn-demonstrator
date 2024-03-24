// Condition is a class used for describing the condition
// that needs to be met before a journey can start. It specifies
// that a traveller must be at a specific location at some given time.
public class Condition {
    public string location;
    public float time;
    public PersistentTraveller traveller;

    public Condition(string location, float time, PersistentTraveller traveller) {
        this.location = location;
        this.time = time;
        this.traveller = traveller;
    }

    public bool ConditionMet(float currentTime) {
        if (currentTime >= this.time && traveller.currentLocation.Equals(this.location)) {
            return true;
        }
        else {
            return false;
        }
    }
}
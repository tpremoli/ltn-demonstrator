using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    List<PersistentTraveller> travellers = new List<PersistentTraveller>();
    public List<Journey> eventList = new List<Journey>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Graph.Instance.IsInitialised);

        for (int i = 0; i < 100; i++) {
            travellers.Add(new PersistentTraveller());
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

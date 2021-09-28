using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public float EVENT_TIME = 30f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Event());
    }

    //Event repeats every EVENT_TIME seconds and picks a random Event from the game world to occur
    private IEnumerator Event()
    {
        WaitForSeconds updateTime = new WaitForSeconds(EVENT_TIME);
        List<Event> events = new List<Event>();

        //Iterate through all possible Events and add to a single possible Event List
        GameObject[] planetObjects = GameObject.FindGameObjectsWithTag("Planet");
        List<Planet> planets = new List<Planet>();

        //Get Planet components from objects
        foreach (GameObject go in planetObjects)
            planets.Add(go.GetComponent<Planet>());
        
        for(int i = 0; i < planets.Count; i++)
        {
            foreach (Event e in planets[i].Events)
            {
                events.Add(e);
            }
        }

        if (events.Count > 0)
            //Randomly execute one of the Events (taking weights into account)
            events[GetRandomWeightedIndex(events)].EventMethod();

        //Sleep until next event checker
        yield return updateTime;
    }

    //Takes a List of Events and returns a random index in the List based on the relative weights of each Event
    private int GetRandomWeightedIndex(List<Event> events)
    {
        //Get the total sum of Event weights
        float weightSum = 0f;
        for (int i = 0; i < events.Count; ++i)
            weightSum += events[i].GetWeight();

        //Iterate through all possibilities and check if each is selected
        int index = 0;
        int lastIndex = events.Count - 1;
        while (index < lastIndex)
        {
            //Do a probability check with a likelihood of weights[index] / weightSum
            if (Random.Range(0, weightSum) < events[index].GetWeight())
                return index;

            //Remove the item's weight from the total weight sum and repeat with the rest
            weightSum -= events[index].GetWeight();
            index++;
        }

        //No other item was selected, so return last index
        return index;
        //(if we want the possibility of no events occurring, could remove the -1 in lastIndex and then return null or something here instead)
    }

}

using UnityEngine;

//An abstract class that represents an Event component
public abstract class Event
{
    private Planet planet;
    private float weight; //the chance for the event to occur (as a ratio. We should probably max this at 100 to keep things simple)

    //This is the method that will run when the event is triggered
    public abstract void EventMethod();

    //Base constructor, receives attached planet as input
    public Event(Planet planet)
    {
        SetPlanet(planet);
    }

    //If the event occurs on a Planet, the event subclass will call this
    public Planet GetPlanet() { return planet; }
    public void SetPlanet(Planet planet) { this.planet = planet; }

    public float GetWeight() { return weight; }
    public void SetWeight(float weight) { this.weight = weight; }

}
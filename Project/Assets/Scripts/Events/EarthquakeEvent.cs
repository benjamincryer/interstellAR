using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeEvent : Event
{
    public EarthquakeEvent(Planet planet) : base(planet)
    {
        SetWeight(10f);
    }

    //Damages target Planet and buildings on it
    public override void EventMethod()
    {
        Planet pl = GetPlanet();
        pl.Health -= 100;
        Debug.Log("Event: Earthquake occurred");
    }

}

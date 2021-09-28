using UnityEngine;

public class SunGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 0;
        MAX_FOOD = 0;
        MIN_ENERGY = 100;
        MAX_ENERGY = 200;
        MIN_METAL = 0;
        MAX_METAL = 0;
        MaterialName = "Star";
        Colour = Color.red;
        //AddEvent(new SolarFlare());
    }
}
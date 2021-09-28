using UnityEngine;

public class RockyPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 3000;
        MAX_FOOD = 5000;
        MIN_ENERGY = 3;
        MAX_ENERGY = 4;
        MIN_METAL = 6;
        MAX_METAL = 8;
        int i = Random.Range(1, 3);
        MaterialName = "Rocky" + i;
        Colour = Color.yellow;
        AddEvent("EarthquakeEvent");

        NameList.Add("Mars");
        NameList.Add("Odin");
        NameList.Add("Agamar");
        NameList.Add("Quixote");
        NameList.Add("Fomalhaut");
        NameList.Add("Dhara");
        NameList.Add("Enten");
        NameList.Add("Nuska");

    }
}
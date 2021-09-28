using UnityEngine;

public class GreenPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 6000;
        MAX_FOOD = 8000;
        MIN_ENERGY = 5;
        MAX_ENERGY = 7;
        MIN_METAL = 3;
        MAX_METAL = 5;
        int i = Random.Range(1, 3);
        MaterialName = "Green" + i;
        Colour = Color.green;

        NameList.Add("Earth");
        NameList.Add("Terra");
        NameList.Add("Gaia");
        NameList.Add("Idunn");
        NameList.Add("Dagobah");
        NameList.Add("Pan");
        NameList.Add("Naiades");
        NameList.Add("Sif");
        NameList.Add("Ostara");
        NameList.Add("Brigid");

    }
}
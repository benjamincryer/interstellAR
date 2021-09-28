using UnityEngine;

public class OceanPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 8000;
        MAX_FOOD = 10000;
        MIN_ENERGY = 5;
        MAX_ENERGY = 6;
        MIN_METAL = 1;
        MAX_METAL = 5;
        MaterialName = "Ocean";
        Colour = Color.cyan;

        NameList.Add("Susanoo");
        NameList.Add("Freyr");
        NameList.Add("Namaka");
        NameList.Add("Wirnpa");
        NameList.Add("Aegir");
        NameList.Add("Nu");
        NameList.Add("Proteus");
        NameList.Add("Selkie");
        NameList.Add("Danu");

    }
}
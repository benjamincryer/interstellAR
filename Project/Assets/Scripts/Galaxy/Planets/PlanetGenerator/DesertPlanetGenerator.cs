using UnityEngine;

public class DesertPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 1000;
        MAX_FOOD = 2000;
        MIN_ENERGY = 6;
        MAX_ENERGY = 8;
        MIN_METAL = 4;
        MAX_METAL = 6;
        MaterialName = "Desert";
        Colour = Color.yellow;

        NameList.Add("Arrakis");
        NameList.Add("Tatooine");
        NameList.Add("Abydos");
        NameList.Add("Venus");
        NameList.Add("Psamathe");
        NameList.Add("Limos");
        NameList.Add("Styx");
        NameList.Add("Lethe");
        NameList.Add("Enma");

    }
}
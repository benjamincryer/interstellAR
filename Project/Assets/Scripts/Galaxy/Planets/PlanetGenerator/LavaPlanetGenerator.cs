using UnityEngine;

public class LavaPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 500;
        MAX_FOOD = 1000;
        MIN_ENERGY = 8;
        MAX_ENERGY = 10;
        MIN_METAL = 1;
        MAX_METAL = 6;
        MaterialName = "Lava";
        Colour = Color.red;

        NameList.Add("Mustafar");
        NameList.Add("Poltergeist");
        NameList.Add("Amaterasu");
        NameList.Add("Agni");
        NameList.Add("Vesta");
        NameList.Add("Ra");
        NameList.Add("Batara-Kala");
        NameList.Add("Aganju");
        NameList.Add("Nuska");

    }
}
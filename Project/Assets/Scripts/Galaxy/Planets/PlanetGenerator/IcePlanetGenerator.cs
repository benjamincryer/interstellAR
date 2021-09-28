using UnityEngine;

public class IcePlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 4000;
        MAX_FOOD = 6000;
        MIN_ENERGY = 4;
        MAX_ENERGY = 6;
        MIN_METAL = 2;
        MAX_METAL = 3;
        int i = Random.Range(1, 3);
        MaterialName = "Ice" + i;
        Colour = Color.white;

        NameList.Add("Hoth");
        NameList.Add("Uranus");
        NameList.Add("Neptune");
        NameList.Add("Tsukuyomi");
        NameList.Add("Loki");
        NameList.Add("Boreas");
        NameList.Add("Khione");
        NameList.Add("Skaoi");
        NameList.Add("Ded-Moroz");

    }
}
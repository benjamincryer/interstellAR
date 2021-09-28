using UnityEngine;

public class IronPlanetGenerator : PlanetGenerator
{
    public override void Init()
    {
        MIN_FOOD = 2000;
        MAX_FOOD = 3000;
        MIN_ENERGY = 2;
        MAX_ENERGY = 3;
        MIN_METAL = 8;
        MAX_METAL = 10;
        MaterialName = "Iron";
        Colour = Color.grey;

        NameList.Add("Mercury");
        NameList.Add("Tyr");
        NameList.Add("Osiris");
        NameList.Add("Ogun");
        NameList.Add("Wasp");
        NameList.Add("Vulcan");
        NameList.Add("Hephaestus");
        NameList.Add("Kagu-Tsuchi");
    }
}
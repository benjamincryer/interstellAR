using System.Collections.Generic;
using UnityEngine;

//This is a utility class, called by Planet script to generate each of its attributes
//Has a subclass for each Planet Type, with different sets of possible attributes to generate based on the T
public abstract class PlanetGenerator
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string MaterialName { get; set; }
    public Color Colour { get; set; }
    public List<string> Events { get; set; }
    public int MAX_HEALTH { get; set; }
    public int MAX_METAL { get; set; }
    public int MAX_ENERGY { get; set; }
    public int MAX_FOOD { get; set; }
    public int MIN_HEALTH { get; set; }
    public int MIN_METAL { get; set; }
    public int MIN_ENERGY { get; set; }
    public int MIN_FOOD { get; set; }
    public List<string> NameList { get; set; }

    public PlanetGenerator()
    {
        Events = new List<string>();
        MAX_HEALTH = 10000;
        MAX_METAL = 5000;
        MAX_ENERGY = 5000;
        MAX_FOOD = 5000;
        MIN_HEALTH = 500;
        MIN_METAL = 1000;
        MIN_ENERGY = 1000;
        MIN_FOOD = 1000;
        NameList = new List<string>();
        //NameList.Add("Planet");

        Init();
    }

    //Used to set attributes, Overridden by child classes
    public abstract void Init();

    //METHODS TO GENERATE EACH OF THE RESOURCE VALUES
    public int GenMetal()
    {
        return Random.Range(MIN_METAL, MAX_METAL);
    }

    public int GenHealth()
    {
        return Random.Range(MIN_HEALTH, MAX_HEALTH);
    }

    public int GenEnergy()
    {
        return Random.Range(MIN_ENERGY, MAX_ENERGY);
    }

    public int GenFood()
    {
        return Random.Range(MIN_FOOD, MAX_FOOD);
    }

    public string GenName()
    {
        //Add a bunch of letters and numbers to the end to make it more sci-fi
        string[] alphabet = new string[26]
            {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};

        //Up to 3 letters can be picked (minimum 1)
        string suffix = "";
        for(int i = 0; i < 3; i++)
        {
            if (i == 0 || Random.value <= 0.33) suffix += alphabet[Random.Range(0, 26)];
        }

        //Randomised name, suffix and number
        return NameList[Random.Range(0, NameList.Count)] + " " + suffix + "-" + Random.Range(0,1000);
    }

    public void AddEvent(string e)
    {
        Events.Add(e);
    }

}
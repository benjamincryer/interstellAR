using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechButton : MonoBehaviour
{

    public void ResearchTech(string tech)
    {
        TechFactory(tech).Research();
    }


    private Technology TechFactory(string tech)
    {
        switch(tech)
        {
            case ("ColonyUpgrade"): return new ColonyUpgradeTech();
        }
        return null;
    }

}

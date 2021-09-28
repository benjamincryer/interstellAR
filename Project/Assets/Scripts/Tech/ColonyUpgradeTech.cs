using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyUpgradeTech : Technology
{
    public override void Init()
    {
        Name = "Colony Upgrade 1";
        Desc = "Increases the size and sturdiness of your Colony.\n" +
            "+5% Max Health\n" + "+10 Citizen Capacity";
        Cost = 5;
    }

    //Called on research complete, applies buff to existing buildings and adds this object to the Player classes' list of researched Techs
    public override void EffectOnResearch()
    {
        Debug.Log("Colony Upgrade bought");
        //foreach(Building b in GetComponentsInChildren<Building>())
        //{

        //}
    }

    //Called whenever a new structure is built
    public override void EffectOnUse()
    {

    }
        
}

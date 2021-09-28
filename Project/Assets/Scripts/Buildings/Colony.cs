using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This building is special, as it also contains Citizens.
public class Colony : Building
{
    private int numCivilians;

    private void Awake()
    {
        InitialiseStats("Colony", 300, 1.25f, 5);
    }

    public override void BuildingAction()
    {
    }

    public override void UpgradeBuilding()
    {
        throw new System.NotImplementedException();
    }


}

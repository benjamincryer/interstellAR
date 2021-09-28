using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Building
{
    private void Awake()
    {
        InitialiseStats("Mine", 375, 1.5f, 30);
    }

    public override void BuildingAction()
    {
        Planet attachedPlanet = GetComponentInParent<Planet>();
        if (attachedPlanet.Metal.ResourceValue > 0)
        {
            //Increase player resources, decrease planet resource
            attachedPlanet.Metal.ResourceValue -= Mathf.RoundToInt(ResourceRate * Time.deltaTime);
            Owner.PlayerInventory.Metal.ResourceValue += Mathf.RoundToInt(ResourceRate * Time.deltaTime);
        }
    }

    public override void UpgradeBuilding()
    {
        throw new System.NotImplementedException();
    }
}
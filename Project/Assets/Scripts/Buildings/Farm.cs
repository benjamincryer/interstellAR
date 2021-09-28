using UnityEngine;

public class Farm : Building
{
    private void Awake()
    {
        InitialiseStats("Farm", 225, 2.5f, 30);
    }

    public override void BuildingAction()
    {
        Planet attachedPlanet = GetComponentInParent<Planet>();
        if (attachedPlanet.Food.ResourceValue > 0)
        {
            //Increase player resources, decrease planet resource
            attachedPlanet.Food.ResourceValue -= Mathf.RoundToInt(ResourceRate * Time.deltaTime);
            Owner.PlayerInventory.Food.ResourceValue += Mathf.RoundToInt(ResourceRate * Time.deltaTime);
        }
    }

    public override void UpgradeBuilding()
    {
        throw new System.NotImplementedException();
    }
}
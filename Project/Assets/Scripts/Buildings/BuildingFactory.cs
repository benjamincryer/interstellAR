using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Factory to instantiate building prefabs
public class BuildingFactory : MonoBehaviour
{
    //public ShipFactory shipFactory;
    public Farm farm;
    public Mine mine;
    public Colony colony;

    public Building CreateBuilding(string building, Vector3 position)
    {
        Building newBuilding = Instantiate(GetBuildingType(building), position, Quaternion.identity);
        return newBuilding;
    }
    public Building GetBuildingType(string building)
    {
        switch (building)
        {
            //case ("ShipFactory"): return shipFactory;
            case ("Farm"): return farm;
            case ("Mine"): return mine;
            case ("Colony"): return colony;
        }
        return null;
    }
}

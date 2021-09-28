using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelector : Selector<Building>
{
    public override void PrintInformation()
    {
        //Print out building information
        if (SelectedObject)
        {
            selectedText.text += 
                "\nBuilding Name: " + SelectedObject.BuildingName + 
                "\nHealth: " + SelectedObject.Health +
                "\nCost: " + SelectedObject.Cost + 
                "\nRepair Rate: " + SelectedObject.RepairRate
                + "\n --------------------";
        }
        SelectedObject = null;
    }
}

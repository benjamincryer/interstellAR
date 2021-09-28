using UnityEngine;
using Photon.Pun;

public class TriangleFormation : Formation
{
    public override void CalculateFormationPositions()
    {
        //Central unit is only unit at row 0, so start from row 1
        int currentRow = 1;
        Transform central = CentralUnit.transform;
        float currentCol = central.localPosition.x - (MinDistance * currentRow);
        foreach (Unit unit in ContainedUnits)
        {
            //Sets central position at the front
            if (unit == CentralUnit)
            {
                unit.Destination = CentralUnit.transform.localPosition;
            }
            else
            {
                //If the distance of a unit from the central position of the current row exceeds the maximum distance
                //I.e. columns/2 is the number of units to the left or right of the central
                Vector3 pos = new Vector3(currentCol, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                Vector3 adjustedCentral = new Vector3(central.localPosition.x, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                //Multiply by 1.01 to account for slight differences in the positions
                if (Vector3.Distance(pos, adjustedCentral) > currentRow * MinDistance * 1.01)
                {
                    currentRow++;
                    currentCol = central.localPosition.x - (currentRow * MinDistance);
                    pos = new Vector3(currentCol, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                }
                unit.Destination = pos;
                currentCol += MinDistance * 2;
            }
        }
    }

    [PunRPC]
    public void RPCSetParent()
    {
        //parent to Units
        transform.parent = GameObject.Find("Units").transform;
    }

    //Will need an RPC for adding and removing from ContainedUnits
    [PunRPC]
    public void RPCAddUnit(int parentID)
    {
        ContainedUnits.Add(PhotonView.Find(parentID).gameObject.GetComponent<Unit>());
    }

    [PunRPC]
    public void RPCRemoveUnit(int parentID)
    {
        ContainedUnits.Remove(PhotonView.Find(parentID).gameObject.GetComponent<Unit>());
    }

    [PunRPC]
    public void RPCRemoveUnitAt(int i)
    {
        ContainedUnits.RemoveAt(i);
    }

    [PunRPC]
    public void RPCDestroy()
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer) PhotonNetwork.Destroy(photonView);
    }
}
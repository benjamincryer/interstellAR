using UnityEngine;
using Photon.Pun;

public class StraightLineFormation : Formation
{
    public override void CalculateFormationPositions()
    {
        if (CentralUnit)
        {
            int unitCount = ContainedUnits.Count;
            Transform central = CentralUnit.transform;
            //Find the number of units to be allocated to each row
            int columns = Mathf.RoundToInt(unitCount / Rows);
            while (Rows * columns < unitCount) { columns++; }
            //Assign position of central unit to calculate other positions from
            CentralUnit.Destination = CentralUnit.transform.localPosition;
            int currentRow = 0;
            //Start positions from the furthest "left" of the central, moving right
            float x = central.localPosition.x - (columns / 2) * MinDistance;
            foreach (Unit unit in ContainedUnits)
            {
                if (x == central.localPosition.x && currentRow == 0)
                {
                    x += MinDistance;
                }
                if (unit != CentralUnit)
                {
                    //Generate a new position
                    Vector3 pos = new Vector3(x, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                    Vector3 adjustedCentral = new Vector3(central.localPosition.x, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                    //If the distance of a unit from the central position of the current row exceeds the maximum distance
                    //I.e. columns/2 is the number of units to the left or right of the central
                    if (Vector3.Distance(pos, adjustedCentral) > (columns / 2) * MinDistance * 1.01)
                    {
                        //If exceeds, move to next row and set position again
                        currentRow++;
                        x = central.localPosition.x - (columns / 2) * MinDistance;
                        pos = new Vector3(x, central.localPosition.y, central.localPosition.z - currentRow * MinDistance);
                    }
                    unit.Destination = pos;
                    x += MinDistance;
                }
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
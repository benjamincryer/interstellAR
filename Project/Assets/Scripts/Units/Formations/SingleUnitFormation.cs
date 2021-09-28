using UnityEngine;
using Photon.Pun;

public class SingleUnitFormation : Formation
{
    public override void CalculateFormationPositions()
    {
        //Only one unit so only needs to be in formation with itself
        if(CentralUnit)
        {
            //CentralUnit.Destination = CentralUnit.transform.position;
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
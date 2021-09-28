using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BattleCruiser : Unit
{
    private void Awake()
    {
        InitialiseStats("Battle Cruiser", 10, 13f, 2);
    }

    [PunRPC]
    private void InitAttributes(int MaxHealth, string UnitName, int Damage, float Speed)
    {
        this.MaxHealth = MaxHealth;
        this.Health = MaxHealth;
        this.UnitName = UnitName;
        this.Damage = Damage;
        this.Speed = Speed;
    }

    [PunRPC]
    private void RPCSetParent(int parentID)
    {
        //Debug.Log(PhotonView.Find(parentID));
        transform.parent = PhotonView.Find(parentID).gameObject.transform;
    }

    [PunRPC]
    public void RPCSetAttackerID(int AttackerID)
    {
        this.AttackerID = AttackerID;
    }

    [PunRPC]
    public void RPCGivePoints(int points)
    {
        //Give player that destroyed the unit points
        if (PhotonNetwork.LocalPlayer.ActorNumber == AttackerID)
        {
            PlayerController.IncreaseScore(points);
        }
    }

    [PunRPC]
    public void RPCDestroy()
    {
        //The owner is responsible for destroying its creation...
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            GameObject.Find("GameController").GetComponent<PlayerController>().EnergyUsed--;
            PhotonNetwork.Destroy(photonView);
        }
    }

}
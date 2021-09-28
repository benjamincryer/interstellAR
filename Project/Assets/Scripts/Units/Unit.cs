using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Unit : MonoBehaviourPun, Upgradeable
{
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public float Speed { get; set; }
    public float RotationSpeed { get; set; }
    public string UnitName { get; set; }
    public Vector3 Destination { get; set; }
    public MathController MathController { get; set; }
    public ResourceFinder ResourceFinder { get; set; }
    public PlayerController Owner { get; set; }
    public List<Upgrade> Upgrades { get; set; }
    public int AttackerID { get; set; }

    public void Start()
    {
        Upgrades = new List<Upgrade>();
        MathController = GameObject.Find("GameController").GetComponent<MathController>();
        ResourceFinder = GameObject.Find("GameController").GetComponent<ResourceFinder>();
        RotationSpeed = 0.5f;

        SetMaterialColour();

        //transform.parent = ResourceFinder.planetsParent.transform;

    }

    public void InitialiseStats(string name, int health, float speed, int damage)
    {
        UnitName = name;
        Health = health;
        Speed = speed;
        Damage = damage;
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
    }

    public void ApplyUpgrades()
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            ApplyUpgrade(Upgrades[i]);
        }
    }

    public void SetMaterialColour()
    {
        if (gameObject.GetPhotonView().Owner.CustomProperties.ContainsKey("colour"))
        {
            int playerColour = (int)gameObject.GetPhotonView().Owner.CustomProperties["colour"];

            //Set Material to player's colour
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Renderer>().material.color = GameManager.ColourList[playerColour];
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetType() == typeof(Sun))
        {
            Instantiate(ResourceFinder.explosionPrefab, transform.position, transform.rotation);
            photonView.RPC("RPCDestroy", RpcTarget.AllBuffered);
        }
    }

    public void InitRPC()
    {
        //Provides all generated attributes to the RPC call - now, all instances of this object on other clients will receive the same attributes
        photonView.RPC("InitAttributes", RpcTarget.AllBuffered, Health, UnitName, Damage, Speed);
    }

    public void SetParent(int parentID)
    {
        photonView.RPC("RPCSetParent", RpcTarget.AllBuffered, parentID as object);
    }

    public void SetAttacker(int attackerID)
    {
        photonView.RPC("RPCSetAttackerID", RpcTarget.AllBuffered, attackerID as object);
    }

    public void GivePoints(int points)
    {
        photonView.RPC("RPCGivePoints", RpcTarget.AllBuffered, points as object);
    }

    public void DestroyUnit()
    {
        photonView.RPC("RPCDestroy", RpcTarget.AllBuffered);
    }

}
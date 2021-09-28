using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour, Upgradeable
{
    public string BuildingName { get; set; }
    public float Maxhealth_base { get; set; }
    public float RepairRate_base { get; set; }
    public float Cost_base { get; set; }
    public float ResourceRate_base { get; set; }
    public float MaxHealth { get; set; }
    public float Health { get; set; }
    public float RepairRate { get; set; }
    public float Cost { get; set; }
    public float ResourceRate { get; set; }
    public PlayerController Owner { get; set; }
    public List<Upgrade> Upgrades { get; set; }
    public bool Repairing { get; set; }
    public bool Destroyed { get; set; }
    public MathController MathController { get; set; }

    public void SetAttribute(AttributeSetMethod m, float v)
    {
        m(v);
    }

    public float GetAttribute(AttributeGetMethod m)
    {
        return m();
    }

    //This whole section is what allows us to dynamically get and set attributes based on string parameters
    //Eg. you want to apply a building upgrade - can pass in the attribute, operator and operand value ("maxhealth","+","100")
    public delegate void AttributeSetMethod(float v);

    public delegate float AttributeGetMethod();

    private AttributeSetMethod AttributeSetFactory(string attribute)
    {
        switch (attribute)
        {
            case ("health"): return SetHealth;
            case ("repair"): return SetRepairRate;
            case ("cost"): return SetCost;
            case ("resources"): return SetResourceRate;
        }
        return null;
    }

    private AttributeGetMethod AttributeGetFactory(string attribute)
    {
        switch (attribute)
        {
            case ("health"): return GetHealth;
            case ("repair"): return GetRepairRate;
            case ("cost"): return GetCost;
            case ("resources"): return GetResourceRate;
        }
        return null;
    }

    //End of the Forbidden Code

    private void Awake()
    {
        MathController = GameObject.Find("GameController").GetComponent<MathController>();
    }

    private void Update()
    {
        BuildingAction();
        if (Health <= 0 || Destroyed)
        {
            Destroy(this);
        }
        if (Repairing)
        {
            Repair();
        }
    }

    public abstract void UpgradeBuilding();

    public abstract void BuildingAction();

    private void Repair()
    {
        //Increase health of building by a set rate each frame
        Health += RepairRate;
    }

    public void InitialiseStats(string name, float health, float repairRate, int resourceRate)
    {
        BuildingName = name;
        Health = health;
        RepairRate = repairRate;
        ResourceRate = resourceRate;
    }

    //UPGRADE METHODS
    public void AddUpgrade(Upgrade upgrade)
    {
        Upgrades.Add(upgrade);
    }

    //Takes an individual Upgrade object, extracts its data and applies the corresponding operation to the given Building attribute
    public void ApplyUpgrade(Upgrade upgrade)
    {
        string attribute = upgrade.Attribute;
        char op = upgrade.Operator;
        float amount = upgrade.Amount;

        //This is one mess of a line of code
        //Long story short, it uses 2 Factory methods to return methods for Setting, Getting and performing an Operation on the requested attribute.
        //(mathController is used for the Operation part of it)
        AttributeSetMethod asm = AttributeSetFactory(attribute);
        float charOp = MathController.OperationCharOp(op, GetAttribute(AttributeGetFactory(attribute)), amount);
        SetAttribute(asm, charOp);
    }

    //Loops through upgrades List
    public void ApplyUpgrades()
    {
        //Apply multiplication first
        for (int i = 0; i < Upgrades.Count; i++)
        {
            if (Upgrades[i].Operator.Equals('+')) ApplyUpgrade(Upgrades[i]);
        }
        //Apply addition second
        for (int i = 0; i < Upgrades.Count; i++)
        {
            if (Upgrades[i].Operator.Equals('*')) ApplyUpgrade(Upgrades[i]);
        }
    }

    public void SetHealth(float health)
    {
        Health = health;
    }

    public void SetResourceRate(float resourceRate)
    {
        ResourceRate = resourceRate;
    }

    public void SetCost(float cost)
    {
        Cost = cost;
    }

    public void SetRepairRate(float repairRate)
    {
        RepairRate = repairRate;
    }

    public float GetHealth()
    {
        return Health;
    }

    public float GetResourceRate()
    {
        return ResourceRate;
    }

    public float GetCost()
    {
        return Cost;
    }

    public float GetRepairRate()
    {
        return RepairRate;
    }
}
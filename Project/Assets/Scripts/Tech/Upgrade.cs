using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a data class that stores Upgrades to specific attributes
public class Upgrade
{
    public string Attribute { get; set; }//Attribute to apply upgrade to
    public char Operator { get; set; }//Operator to apply to attribute
    public float Amount { get; set; }//Operand

    public Upgrade(string attribute, char op, float amount)
    {
        Attribute = attribute;
        Operator = op;
        Amount = amount;
    }
}

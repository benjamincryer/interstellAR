using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeItem
{
    public int UnitPrice { get; set; }
    public string ItemName { get; set; }
    public int QuantitySelected { get; set; }
    public int QuantityAvailable { get; set; }
    public Text QuantityText { get; set; }
    public Text TotalPriceText { get; set; }

    public TradeItem(int unitPrice, string itemName, int quantityAvailable)
    {
        UnitPrice = unitPrice;
        ItemName = itemName;
        QuantityAvailable = quantityAvailable;
        QuantitySelected = 0;
    }
}
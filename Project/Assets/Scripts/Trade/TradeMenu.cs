using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    public Text metalQuantityText;
    public Text energyQuantityText;
    public Text metalTotalPriceText;
    public Text energyTotalPriceText;
    public Button metalPlusButton;
    public Button metalMinusButton;
    public Button energyPlusButton;
    public Button energyMinusButton;
    public GameObject GameController { get; set; }
    public TradeItem Metal { get; set; }
    public TradeItem Energy { get; set; }

    //This is, for now, set in the Player class' start function
    public PlayerController Player { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        GameController = GameObject.Find("GameController");
        Player = GameObject.Find("GameController").GetComponent<PlayerController>();
        Metal = new TradeItem(10, "Metal", 0);
        Energy = new TradeItem(3000, "Energy", 0);
        Metal.TotalPriceText = metalTotalPriceText;
        Metal.QuantityText = metalQuantityText;
        Energy.TotalPriceText = energyTotalPriceText;
        Energy.QuantityText = energyQuantityText;
        metalPlusButton.interactable = true;
        energyPlusButton.interactable = true;  
    }

    private void Update()
    {
        if (Metal.UnitPrice*500 > Player.PlayerInventory.Credit.ResourceValue)
            metalPlusButton.interactable = false;

        if (Energy.UnitPrice > Player.PlayerInventory.Credit.ResourceValue)
            energyPlusButton.interactable = false;

        if(Metal.QuantitySelected == 0)
            metalMinusButton.interactable = false;

        if (Energy.QuantitySelected == 0)
            energyMinusButton.interactable = false;
    }

    public void SetVisible()
    {
        gameObject.SetActive(true);
    }

    //Button function for incrementing the quantity selected for the item passed in
    public void IncreaseQuantityButtonClick(string itemName)
    {
        switch (itemName)
        {
            case "Metal": UpdateItemQuantity(Metal, 500); break;
            case "Energy": UpdateItemQuantity(Energy, 1); break;
        }
    }

    //Button function for decrementing the quantity selected for the item passed in
    public void DecreaseQuantityButtonClick(string itemName)
    {
        switch (itemName)
        {
            case "Metal": UpdateItemQuantity(Metal, -500); break;
            case "Energy": UpdateItemQuantity(Energy, -1); break;
        }
    }

    //Increase/decreases the given item's selected quantity, total price and corresponding texts
    public void UpdateItemQuantity(TradeItem item, int amount)
    {
        Button plusButton = null, minusButton = null;
        switch (item.ItemName)
        {
            case "Metal": plusButton = metalPlusButton; minusButton = metalMinusButton;break;
            case "Energy": plusButton = energyPlusButton; minusButton = energyMinusButton; break;
        }
        //Increase the selected amount of the item by the amount passed in
        item.QuantitySelected += amount;

        //Prevent negative quantity
        if (item.QuantitySelected < 0) { item.QuantitySelected = 0; }

        //If 1 or more are selected, allow the player to decrease the amount selected using the minus button
        if (item.QuantitySelected > 1)
        {
            minusButton.interactable = true;
        }

        //Calculate total price of the quantity selected
        int totalPrice = item.QuantitySelected * item.UnitPrice;

        //Update text for the total price and quantity selected
        item.TotalPriceText.text = totalPrice.ToString();
        item.QuantityText.text = item.QuantitySelected.ToString();
        Debug.Log("Item total - " + item.TotalPriceText.text + " Item quantity - " + item.QuantityText.text);

        //If the player can't afford to buy another item, prevent them from doing so
        if (totalPrice + item.UnitPrice > Player.PlayerInventory.Credit.ResourceValue)
        {
            plusButton.interactable = false;
        }
        else
        {
            plusButton.interactable = true;
        }

        //Debug.Log("SUCCESSFUL TRADE INTERACTION");
    }

    //Button function to select the appropriate purchase function
    public void PurchaseItemButtonClick(string itemName)
    {
        switch (itemName)
        {
            case "Metal": PurchaseItem(Metal); break;
            case "Energy": PurchaseItem(Energy); break;
        }
    }

    //Purchases the quantity of the passed item selected, hides the purchase panel
    public void PurchaseItem(TradeItem item)
    {
        int totalCost = int.Parse(item.TotalPriceText.text);
        if (Player.PlayerInventory.Credit.ResourceValue >= totalCost)
        {
            Player.PlayerInventory.ChangeQuantity("Credit", -totalCost);
            Player.PlayerInventory.ChangeQuantity(item.ItemName, item.QuantitySelected);
        }

        //Reset purchase menu
        item.TotalPriceText.text = "0";
        item.QuantityText.text = "0";
        item.QuantityAvailable -= item.QuantitySelected;
        item.QuantitySelected = 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalPlus : MonoBehaviour
{
    private Text number;
    private Text totalPrice;
    private int unitPriceInt;
    private int currentMoney;

    public void Start()
    {
        unitPriceInt = int.Parse(GameObject.Find("Canvas/metal_shoppanel/Price_text").GetComponent<Text>().text);
        currentMoney = int.Parse(GameObject.Find("Canvas/Panel/current_money/Text").GetComponent<Text>().text);
        this.GetComponent<Button>().interactable = true;
    }

    public void ChangeNumber()
    {
        number = GameObject.Find("Canvas/metal_shoppanel/Num_text").GetComponent<Text>();
        int numberInt = int.Parse(number.text);
        numberInt++;
        if (numberInt > 1)
        {
            GameObject.Find("Canvas/metal_shoppanel/Minus_butt").GetComponent<Button>().interactable = true;
        }
        totalPrice = GameObject.Find("Canvas/metal_shoppanel/Price_text").GetComponent<Text>();
        int totalPriceInt = int.Parse(totalPrice.text);
        totalPriceInt += unitPriceInt;
        totalPrice.text = totalPriceInt.ToString();
        number.text = numberInt.ToString();
        if ((totalPriceInt + unitPriceInt) > currentMoney)
        {
            this.GetComponent<Button>().interactable = false;
        }
        else
            this.GetComponent<Button>().interactable = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyMinus : MonoBehaviour
{
    private Text number;
    private Text totalPrice;
    private int unitPriceInt;
    public void Start()
    {
        unitPriceInt = int.Parse(GameObject.Find("Canvas/energy_shoppanel/Price_text").GetComponent<Text>().text);
        this.GetComponent<Button>().interactable = false;
    }

    public void changeNumber()
    {
        number = GameObject.Find("Canvas/energy_shoppanel/Num_text").GetComponent<Text>();
        int numberInt = int.Parse(number.text);
        numberInt--;
        number.text = numberInt.ToString();
        totalPrice = GameObject.Find("Canvas/energy_shoppanel/Price_text").GetComponent<Text>();
        int totalPriceInt = int.Parse(totalPrice.text);
        totalPriceInt -= unitPriceInt;
        totalPrice.text = totalPriceInt.ToString();
        if (numberInt > 1)
        {
            this.GetComponent<Button>().interactable = true;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
        }

        GameObject.Find("Canvas/energy_shoppanel/Plus_butt").GetComponent<Button>().interactable = true;

    }
}

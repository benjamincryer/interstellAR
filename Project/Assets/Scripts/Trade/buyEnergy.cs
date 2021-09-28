using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buyEnergy : MonoBehaviour
{
    public GameObject Panel;
    public Text currentMoney;
    public Text itemValue;
    public void reduceMoney()
    {
        currentMoney = GameObject.Find("Canvas/Panel/current_money/Text").GetComponent<Text>();
        itemValue = GameObject.Find("Canvas/energy_shoppanel/Price_text").GetComponent<Text>();
        int money = int.Parse(currentMoney.text);
        if (money >= int.Parse(itemValue.text))
        {
            money = money - int.Parse(itemValue.text);
            currentMoney.text = money.ToString();
        }
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }
}

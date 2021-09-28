using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeMoney : MonoBehaviour
{
    public Text Money;
    public UpgradeResource playerMoney;
    public string PlayerName { get; set; }
    public PlayerInventory PlayerInventory { get; set; }
    public GameObject PlayerObject { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        GetPlayerMoney(GameObject.Find("Player 0").GetComponent<PlayerInventory>());
    }

    private void GetPlayerMoney(PlayerInventory playerInventory)
    {
        Money.text = playerInventory.Credit.ResourceValue.ToString();
    }
}
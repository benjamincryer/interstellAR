using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private static int STARTCRED = 10000, STARTMETAL = 1000, STARTFOOD = 250,
        STARTENERGY = 100, STARTTECH = 1, STARTINFLUENCE = 10, STARTINVENTORYSIZE = 10000;

    public int InventorySize { get; set; }
    public UpgradeResource Metal { get; set; }
    public UpgradeResource Credit { get; set; }
    public UpgradeResource Tech { get; set; }
    public UpkeepResource Food { get; set; }
    public UpkeepResource Energy { get; set; }
    public UpkeepResource Influence { get; set; }


    private void Start()
    {
        Metal = gameObject.AddComponent<UpgradeResource>();
        Metal.SetupResource("Metal", STARTMETAL, 0);
        Credit = gameObject.AddComponent<UpgradeResource>();
        Credit.SetupResource("Credit", STARTCRED, 0);
        Tech = gameObject.AddComponent<UpgradeResource>();
        Tech.SetupResource("Tech", STARTTECH, 0);
        Food = gameObject.AddComponent<UpkeepResource>();
        Food.SetupResource("Food", STARTFOOD, 0);
        Energy = gameObject.AddComponent<UpkeepResource>();
        Energy.SetupResource("Energy", STARTENERGY, 0);
        Influence = gameObject.AddComponent<UpkeepResource>();
        Influence.SetupResource("Influence", STARTINFLUENCE, 0);
        InventorySize = STARTINVENTORYSIZE;
    }

    private void Update()
    {
    }

    public void ChangeQuantity(string resourceName, int amount)

    {
        switch (resourceName)
        {
            case "Metal":
                Metal.ResourceValue += amount; break;
            case "Energy":
                Energy.ResourceValue += amount; break;
            case "Food":
                Food.ResourceValue += amount; break;
            case "Credit":
                Credit.ResourceValue += amount; break;
            case "Tech":
                Tech.ResourceValue += amount; break;
            case "Influence:":
                Influence.ResourceValue += amount; break;
        }
    }

    public void ZeroEnergy()
    {
        Energy.ResourceValue = 0;
    }

   
}
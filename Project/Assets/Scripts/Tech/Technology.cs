using UnityEngine;

//This class represents a technology, with an associated cost and an Effect method that triggers on purchase
public abstract class Technology : MonoBehaviour
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Cost { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    //Called when the Research button is pressed
    public void Research()
    {
        PlayerController p = FindObjectOfType<PlayerController>();
        Resource tech = p.PlayerInventory.Tech;
        //Deduct tech points and apply tech effect IF player has enough points
        if (tech.ResourceValue >= Cost)
        {
            tech.ResourceValue -= Cost;
            EffectOnResearch();
        }
    }

    //Used to set attributes, Overridden by child classes
    public abstract void Init();

    //Used to define the tech's effect upon being researched (eg. apply upgrades to existing buildings), Overridden by child classes
    public abstract void EffectOnResearch();

    //Used to define the tech's effect on use (eg. apply upgrades to any newly-created buildings), Overridden by child class
    public abstract void EffectOnUse();
}
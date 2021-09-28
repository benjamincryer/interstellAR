using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class Planet : HeavenlyObject
{
    public static string NULLOCCUPIER = "None", NULLPLANETNAME = "Unknown";

    public GameObject BuildingObjects;
    public float Health { get; set; }
    public string PlanetName { get; set; }
    public bool Occupied { get; set; }
    public UpgradeResource Metal { get; set; }
    public UpkeepResource Energy { get; set; }
    public UpkeepResource Food { get; set; }
    public string PlanetMat { get; set; }
    public Color AtmosColor { get; set; }
    public List<Event> Events { get; set; }
    public ResourceFinder ResourceFinder { get; set; }
    public float CapturePoints { get; set; } //Current state of capture (at 100 when a player owns the Planet)
    public int CaptureID { get; set; } //Photon ID of the Player currently capturing this Planet
    public Formation OccupyingFormation { get; set; }
    private Color FlagColor { get; set; }

    private void Awake()
    {
        //Assign random values for health/resources for a planet temporarily
        Health = Random.Range(1, 100);
        Metal = gameObject.AddComponent<UpgradeResource>();
        Metal.SetupResource("Metal", Random.Range(1, 1000), 10f);
        Energy = gameObject.AddComponent<UpkeepResource>();
        Energy.SetupResource("Energy", Random.Range(1, 1000), 1f);
        Food = gameObject.AddComponent<UpkeepResource>();
        Food.SetupResource("Food", Random.Range(1, 1000), 1f);

        PlanetName = "Planet " + Metal.ResourceValue + Energy.ResourceValue + Food.ResourceValue;

        Mass = Metal.ResourceValue + Energy.ResourceValue;
        Events = new List<Event>();
        RotationSpeed = rotationConstant / Mass;
        OrbitObject = GameObject.Find("Sun");
        OrbitSpeed = orbitConstant;

        CapturePoints = 0;
        CaptureID = -1;

        MathController = GameObject.Find("GameController").GetComponent<MathController>();
        ResourceFinder = GameObject.Find("GameController").GetComponent<ResourceFinder>();
    }

    private void Start()
    {
        //Set halo size
        float radius = MathController.CalculateRadiusSphere(gameObject);
        if (BuildingObjects) GetComponent<Light>().range = radius*2 + 3;
    }

    private void Update()
    {
        transform.Rotate(0, Time.deltaTime * RotationSpeed, 0);
        CheckCaptured();
        //OrbitClosestSun();
        //Orbit();
    }

    public void InitRPC()
    {
        //Provides all generated attributes to the RPC call - now, all instances of this object on other clients will receive the same attributes
        photonView.RPC("InitAttributes", RpcTarget.AllBuffered, Health, PlanetName, Metal.ResourceValue, Energy.ResourceValue, PlanetMat);
    }

    [PunRPC]
    private void InitAttributes(float Health, string PlanetName, float Metal, float Energy, string PlanetMat)
    {
        this.Health = Health;
        this.PlanetName = PlanetName;
        this.Metal.ResourceValue = Metal;
        this.Energy.ResourceValue = Energy;
        GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Planets/" + PlanetMat);

        Mass = Metal + Energy;

        //parent to Planets
        transform.parent = GameObject.Find("Planets").transform;

        //Debug.Log(info.Sender);
    }

    [PunRPC]
    private void InitAttributesShort(float Health, string PlanetName, float Metal, float Energy)
    {
        this.Health = Health;
        this.PlanetName = PlanetName;
        this.Metal.ResourceValue = Metal;
        this.Energy.ResourceValue = Energy;

        Mass = Metal + Energy;

        //parent to Planets
        transform.parent = GameObject.Find("Planets").transform;
    }

    [PunRPC]
    public void SetCaptureID(int id)
    {
        CaptureID = id;
    }

    [PunRPC]
    public void ChangeCapturePoints(float amount)
    {
        CapturePoints += amount;
    }

    [PunRPC]
    public void SetCapturePoints(float amount)
    {
        CapturePoints = amount;
    }

    [PunRPC]
    public void SetOccupyingFormation(int formID)
    {
        //Debug.Log("Setting occupying formation");
        OccupyingFormation = PhotonView.Find(formID).GetComponent<Formation>();
    }

    [PunRPC]
    public void FreeOccupyingFormation()
    {
        //Debug.Log("Freeing occupying formation");
        OccupyingFormation = null;
    }

    private void OrbitClosestSun()
    {
        Sun[] suns = FindObjectsOfType<Sun>();
        foreach (Sun sun in suns)
        {
            Vector3 sunPosition = sun.transform.position;
            sunPosition.x += sun.transform.localScale.x;
            sunPosition.z += sun.transform.localScale.z;
            if (OrbitObject)
            {
                if (Vector3.Distance(sunPosition, transform.position) < Vector3.Distance(OrbitObject.transform.position, transform.position))
                {
                    OrbitObject = sun.gameObject;
                }
            }
        }
    }

    //This checks if we are at the threshold for transferring control (ie. capture points below 0 or at 100)
    private void CheckCaptured()
    {
        //If below zero capture points at any point, transfer ownership to capturing player
        if (CapturePoints < 0)
        {
            photonView.TransferOwnership(CaptureID);

            //Occupying Formation is removed
            photonView.RPC("FreeOccupyingFormation", RpcTarget.AllBuffered);

            //Gain points
            PlayerController.IncreaseScore(25);
            CapturePoints = 0;
        }

        //If at 100 capture points, stop capturing
        if (CapturePoints >= 100)
        {
            CapturePoints = 100;
        }

        //Enable buildings if planet is owned
        if (photonView.OwnerActorNr != 0 && BuildingObjects != null)
        {
            BuildingObjects.SetActive(true);

            //Set colour to owner
            if (BuildingObjects && photonView.Owner.CustomProperties.ContainsKey("colour"))
            {
                int playerColour = (int)photonView.Owner.CustomProperties["colour"];

                //Set Flag Material to Owner player's colour
                var flag = BuildingObjects.transform.Find("Flag");
                flag.GetChild(0).gameObject.GetComponent<Renderer>().material.color = GameManager.ColourList[playerColour];
            }
        }
            
    }

    public void SetEvents(List<string> events) //get event list from generator object
    {
        for (int i = 0; i < events.Count; i++)
        {
            Events.Add(EventFactory(events[i]));
        }
    }

    public void UpdateColor(Color c)
    {
        AtmosColor = c;
        GetComponent<Light>().color = AtmosColor;
    }

    public void SetSize(float radius)
    {
        transform.localScale = new Vector3(radius, radius, radius);
        GetComponent<Light>().range = (float)(radius * 6);
    }

    public void MineAllResources(PlayerInventory inv)
    {
        //(Mining rate increases the higher your ownership)
        float rate = (CapturePoints / 100) * Metal.ResourceRate;
        if (rate < 1) rate = 1;

        /*
        if (Metal.ResourceValue >= Metal.ResourceRate)
        {
            inv.Metal.ResourceValue += rate;
            Metal.ResourceValue -= rate;
        }
        else if (Metal.ResourceValue > 0)
        {
            inv.Metal.ResourceValue += Metal.ResourceValue;
            Metal.ResourceValue = 0;
        }*/

        //Metal
        inv.Metal.ResourceValue += Metal.ResourceValue;
        
        //Energy
        inv.Energy.ResourceValue += Energy.ResourceValue;

    }

    public Event EventFactory(string e)
    {
        switch (e)
        {
            case ("EarthquakeEvent"): return new EarthquakeEvent(this);
        }
        return null;
    }
}
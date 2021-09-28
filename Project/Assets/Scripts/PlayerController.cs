using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerController : MonoBehaviourPun
{
    public GameObject formationTextPrefab;
    public GameObject PlanetsParent { get; set; }
    public Text PlayerResource { get; set; }
    public ResourceFinder ResourceFinder { get; set; }
    public List<Planet> ControlledPlanets { get; set; }
    public List<Building> ControlledBuildings { get; set; }
    public List<Planet> SelectedPlanets { get; set; }
    public List<Formation> SelectedFormations { get; set; }
    public MathController MathController { get; set; }
    public string PlayerName { get; set; }
    public PlayerInventory PlayerInventory { get; set; }
    public GameObject PlayerObject { get; set; }
    public FormationManager FM { get; set; }
    public float SecondsToMine { get; set; }
    public GameObject PlanetPanel { get; set; }
    public GameObject EnemyPanel { get; set; }
    public GameObject FormationPanel { get; set; }
    public GameObject ShopPanel { get; set; }
    public GameObject ScorePanel { get; set; }
    public GameObject WinPanel { get; set; }
    public GameObject Audio { get; set; }
    public int EnergyUsed { get; set; }
    public Text CreditText { get; set; }
    public Text MetalText { get; set; }
    public Text EnergyText { get; set; }

    int scoreTally { get; set; }
    public double GameLength { get; set; }
    public bool GameOver = false;

    private GameObject[] PlanetFunctions { get; set; }

    public void Awake()
    {
        MathController = GameObject.Find("GameController").GetComponent<MathController>();
        ResourceFinder = GameObject.Find("GameController").GetComponent<ResourceFinder>();
        PlanetPanel = GameObject.FindGameObjectWithTag("PlanetPanel");
        EnemyPanel = GameObject.FindGameObjectWithTag("EnemyPanel");
        FormationPanel = GameObject.FindGameObjectWithTag("FormationsPanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        ScorePanel = GameObject.FindGameObjectWithTag("ScorePanel");
        WinPanel = GameObject.FindGameObjectWithTag("WinPanel");
        Audio = GameObject.FindGameObjectWithTag("UIAudio");
        CreditText = GameObject.FindGameObjectWithTag("CreditResource").GetComponent<Text>();
        MetalText = GameObject.FindGameObjectWithTag("MetalResource").GetComponent<Text>();
        EnergyText = GameObject.FindGameObjectWithTag("EnergyResource").GetComponent<Text>();

        WinPanel.SetActive(false);

        PlayerObject = new GameObject(PlayerName);
        ControlledPlanets = new List<Planet>();
        ControlledBuildings = new List<Building>();
        InitialiseSelectors();

        //10 mins
        GameLength = 600;
    }

    public void Start()
    {
        //Get tags b4 they go invisible
        PlanetFunctions = GameObject.FindGameObjectsWithTag("PlanetFunctions");

        EnergyUsed = 0;
        scoreTally = 0;

        PlayerInventory = PlayerObject.AddComponent<PlayerInventory>();
        FM = PlayerObject.AddComponent<FormationManager>();
        PlayerResource = ResourceFinder.playerResourceText;
        PlanetsParent = ResourceFinder.planetsParent;
        FM.Player = this;
        SecondsToMine = 1f;
        InvokeRepeating("DoMining", 0f, SecondsToMine);
    }

    private void Update()
    {
        SelectedFormations = PlayerObject.GetComponent<FormationSelector>().SelectedObjects;
        SelectedPlanets = PlayerObject.GetComponent<PlanetSelector>().SelectedObjects;
        Controls();
        UpdateSelectedPlanetPanel();
        UpdateSelectedEnemyPanel();
        PrintResources();
        
        if (!GameOver) DisplayPlayerScores();
    }

    //Initialises the selector components for the player
    public void InitialiseSelectors()
    {
        SelectedPlanets = new List<Planet>();
        SelectedFormations = new List<Formation>();
        PlayerObject.AddComponent<UnitSelector>();
        var planetSel = PlayerObject.AddComponent<PlanetSelector>();
        var formSel = PlayerObject.AddComponent<FormationSelector>();
        PlayerObject.GetComponent<PlanetSelector>().selectShader = ResourceFinder.selectShader;
        //PlayerObject.GetComponent<PlanetSelector>().selectedText = ResourceFinder.planetSelectedText;
        PlayerObject.GetComponent<FormationSelector>().selectShader = ResourceFinder.selectShader;
        //PlayerObject.GetComponent<FormationSelector>().selectedText = ResourceFinder.formationSelectedText;

        //Attach selectors to deselect button
        var deselect = GameObject.Find("DeselectButton").GetComponent<Button>();
        deselect.onClick.AddListener(delegate { formSel.DeselectAll(); });
        deselect.onClick.AddListener(delegate { planetSel.DeselectAll(); });

        var move = GameObject.Find("MoveToButton").GetComponent<Button>();
        deselect.onClick.AddListener(delegate { MoveAllFormations(); });
    }

    private void Controls()
    {
        //If movement input, should move all units to chosen destination
        if (Input.GetKeyDown(KeyCode.F))
        {
            FM.CreateUnitFormation("StraightLineFormation");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            MoveAllFormations();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            FM.DisbandAllFormations();
        }
        /*
        else if (Input.GetKeyDown(KeyCode.E))
        {
            CreateUnit("AttackFighter");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            CreateUnit("Destroyer");
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            CreateUnit("BattleCruiser");
        }
        */
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AttackSelectedFormation();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerObject.GetComponent<FormationSelector>().SelectAll();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerObject.GetComponent<FormationSelector>().DeselectAll();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeResourceRate("Metal", 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeResourceRate("Food", 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeResourceRate("Energy", 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            foreach (Planet planet in SelectedPlanets)
            {
                SetPlanetOwner(planet);
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            TradeMenu menu = FindObjectOfType<TradeMenu>();
            menu.SetVisible();
        }
    }

    //Enables movement for all units in all selected formations
    public void MoveAllFormations()
    {
        foreach (Formation form in SelectedFormations)
        {
            if (form.Owner == this)
            {
                if (SelectedPlanets.Count > 0)
                {
                    //Idle->Engine sound
                    var moveSound = form.gameObject.transform.GetChild(0).gameObject;
                    var idleSound = form.gameObject.transform.GetChild(1).gameObject;

                    moveSound.GetComponent<AudioSource>().Play();
                    idleSound.GetComponent<AudioSource>().Stop();

                    form.FormationDestination = SelectedPlanets[0].gameObject;
                    form.Move = true;

                    //If we're moving, then we can no longer be a Planet's OccupyingFormation
                    //So check if the formation is an OccupyingFormation and free it
                    var planets = GameObject.FindObjectsOfType<Planet>();
                    foreach(var planet in planets)
                    {
                        if (planet.OccupyingFormation == form)
                        {
                            planet.gameObject.GetPhotonView().RPC("FreeOccupyingFormation", RpcTarget.AllBuffered);
                        }
                    }
                }
                form.Attack = false;
            }
        }
    }

    //Sends all selected, owned formations to attack the enemy formation selected
    public void AttackSelectedFormation()
    {
        if (SelectedFormations.Count >= 2)
        {
            Formation formationToAttack = SelectedFormations[0];

            //Check which of your selected formation is an enemy, set formation to attack
            foreach (Formation form in SelectedFormations)
            {
                if (form.gameObject.GetPhotonView().Owner != PhotonNetwork.LocalPlayer)
                {
                    formationToAttack = form;
                    break;
                }
            }
            //Get formations that you own, set them to attack the formationToAttack
            foreach (Formation form in SelectedFormations)
            {
                if (form.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
                {
                    form.FormationToAttack = formationToAttack;
                    form.Attack = true;
                }
            }
        }
    }

    //Coroutine implementation to do mining evert SecondsToMine seconds
    private void DoMining()
    {
        PlayerInventory.Energy.ResourceValue = 0;
        //PlayerInventory.Metal.ResourceValue = 0;
        MineAllPlanets();

    }

    //Iterates through all controlled planets, and mines all resources from that planet
    private void MineAllPlanets()
    {
        scoreTally = 0;

        Planet[] planets = FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            if (planet.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
            {
                planet.MineAllResources(PlayerInventory);

                //Gain 1 point for every second we control a Planet
                if (planet.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer) scoreTally += 1;
            }
        }

        //Hijacking this function to also tally score
        IncreaseScore(scoreTally);
    }

    public void CreateUnit(string unitName)
    {
        //Calculate spawn position (in front of player)
        /*
        float spawnDistance = 10;
        Vector3 spawnPos = Camera.main.transform.position + (spawnDistance * Camera.main.transform.forward);
        */

        //Create unit on top of planet
        var pos = SelectedPlanets[0].transform.position;
        float radius = MathController.CalculateRadiusSphere(SelectedPlanets[0].gameObject);

        //Creating some distance from other units. if units spawn too close the formation position calculation doesn't work...
        //i wrote this while tired,so is low effort
        float dist;
        if (SelectedPlanets[0].OccupyingFormation == null)
            dist = 0;
        else if (SelectedPlanets[0].OccupyingFormation.ContainedUnits.Count == 1)
            dist = 1f;
        else
            dist = Random.Range(1f, 5f); //unlikely that they'll collide unless you're spam-clicking like a madman


        Vector3 spawnPos = new Vector3(pos.x, pos.y + radius + 2 + dist, pos.z);

        //Creates a unit, then assigns it to a single unit formation
        Unit unit = UnitFactory(unitName, spawnPos);

        if (unit == null)
        {
            Audio.transform.Find("AudioFail").gameObject.GetComponent<AudioSource>().Play();
            return;
        }
        else
        {
            Audio.transform.Find("AudioSuccess").gameObject.GetComponent<AudioSource>().Play();
        }

        //(Unit Factory instantiates and sets owner id automatically)
        unit.Owner = this;
        EnergyUsed++;

        //Creates formation and adds it to controlled formations
        List<Unit> unitList = new List<Unit>();

        //Add new Formation to SelectedPlanet's OccupyingFormation
        if (SelectedPlanets[0].OccupyingFormation != null)
        {
            Debug.Log("Adding to planet's occupying formation");

            //Get units in occupying formation
            unitList = SelectedPlanets[0].OccupyingFormation.ContainedUnits;
            unitList.Add(unit);

            int occForm = FM.CreateUnitFormation("StraightLineFormation", unitList);
            SelectedPlanets[0].gameObject.GetPhotonView().RPC("SetOccupyingFormation",RpcTarget.AllBuffered, occForm as object);
        }
        //Set occupying formation to the newly-created formation
        else
        {
            Debug.Log("Setting new occupying formation");

            //Make new list for new Unit
            unitList.Add(unit);

            int newForm = FM.CreateUnitFormation("SingleUnitFormation", unitList);
            SelectedPlanets[0].gameObject.GetPhotonView().RPC("SetOccupyingFormation", RpcTarget.AllBuffered, newForm as object);
        }

        //Gain score when you create a Unit
        IncreaseScore(10);
    }

    public Unit UnitFactory(string unit, Vector3 position)
    {
        //Do we have enough energy
        if(EnergyUsed >= PlayerInventory.Energy.ResourceValue)
        {
            return null;
        }

        //Check if we can afford it first
        switch(unit)
        {
            case ("AttackFighter"): if (PlayerInventory.Metal.ResourceValue < 250) return null; else PlayerInventory.Metal.ResourceValue -= 250; break;
            case ("BattleCruiser"): if (PlayerInventory.Metal.ResourceValue < 500) return null; else PlayerInventory.Metal.ResourceValue -= 500; break;
            case ("Destroyer"): if (PlayerInventory.Metal.ResourceValue < 750) return null; else PlayerInventory.Metal.ResourceValue -= 750; break;
        }

        Unit newUnit = PhotonNetwork.Instantiate("Prefabs/Ships/" + unit, position, Quaternion.identity).GetComponent<Unit>();
        newUnit.InitRPC();
        return newUnit;
    }

    private void CreateBuilding(string buildingName)
    {
        Planet firstSelectedPlanet = SelectedPlanets[0];
        //0-North, 1-East, 2-South, 3-West
        if (firstSelectedPlanet)
        {
            List<Vector3> planetPoints = MathController.CalculateSpherePoints(firstSelectedPlanet.gameObject);
            Vector3 buildingPosition;
            switch (buildingName)
            {
                case "ShipFactory": buildingPosition = planetPoints[0]; break;
                case "Farm": buildingPosition = planetPoints[1]; break;
                case "Mine": buildingPosition = planetPoints[2]; break;
                case "Colony": buildingPosition = planetPoints[3]; break;
                default: buildingPosition = new Vector3(0, 0, 0); break;
            }
            //This works, but need to update Selector to allow the selected objects to be of type T, instead of gameobject
            Building newBuilding = GameObject.Find("GameController").GetComponent<BuildingFactory>().CreateBuilding(buildingName, buildingPosition);
            ControlledBuildings.Add(newBuilding);
            newBuilding.Owner = this;
            //Make the building a child of the planet
            newBuilding.transform.parent = firstSelectedPlanet.transform;
        }
    }

    //Sets the owner of a planet to this player instance
    public void SetPlanetOwner(Planet planet)
    {
        //planet.Occupier = this;
        planet.Occupied = true;

        planet.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        ControlledPlanets.Add(planet);
    }

    //Increase/decrease the rate of the desired resource by the passed in amount
    public void ChangeResourceRate(string resourceName, float amount)
    {
        Planet selectedPlanet = SelectedPlanets[0];
        //if (selectedPlanet && selectedPlanet.Occupier == this)

        if (selectedPlanet && selectedPlanet.photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Resource[] rs = selectedPlanet.GetComponents<Resource>();
            foreach (Resource r in rs)
            {
                if (r.ResourceName == resourceName)
                {
                    r.ResourceRate += amount;
                }
            }
        }
    }

    //Enables/disables the selected enemy panel if the player has selected a planet
    private void UpdateSelectedPlanetPanel()
    {
        if (PlanetPanel)
        {
            if (SelectedPlanets.Count > 0)
            {
                //Update text to relevant values
                PlanetPanel.SetActive(true);
                Text[] texts = PlanetPanel.GetComponentsInChildren<Text>();
                texts[0].text = SelectedPlanets[0].PlanetName;
                texts[1].text = "Metals: " + SelectedPlanets[0].Metal.ResourceValue.ToString();
                texts[2].text = "Energy: " + SelectedPlanets[0].Energy.ResourceValue.ToString();

                var name = "None"; if (SelectedPlanets[0].gameObject.GetPhotonView().Owner != null) name = SelectedPlanets[0].gameObject.GetPhotonView().Owner.NickName; //set owner to photon owner (if claimed)
                texts[3].text = "Owner: " + name;

                texts[4].text = SelectedPlanets[0].CapturePoints + "%";

                //Enable unit creation only if owned by local player
                foreach (GameObject f in PlanetFunctions)
                {
                    if (SelectedPlanets[0].gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
                    {
                        f.SetActive(true);
                    }
                    else
                    {
                        f.SetActive(false);
                    }
                }

            }
            else
            {
                PlanetPanel.SetActive(false);
                ShopPanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("PLANET PANEL NOT FOUND");
        }
    }

    public void OpenTrade()
    {
        ShopPanel.SetActive(true);
    }

    //Enables/disables the selected enemy panel if the player has selected an enemy
    private void UpdateSelectedEnemyPanel()
    {
        if (EnemyPanel)
        {
            if (SelectedFormations.Count > 0)
            {
                Formation selectedEnemy = PlayerObject.GetComponent<FormationSelector>().CheckSelectedEnemyLimit();
                if (selectedEnemy != null)
                {
                    //Update text to relevant values
                    EnemyPanel.SetActive(true);
                    Text[] texts = PlanetPanel.GetComponentsInChildren<Text>();
                    texts[0].text = "Name: " + selectedEnemy.FormationName;
                    texts[1].text = "Health: " + selectedEnemy.Health;
                    texts[2].text = "Damage: " + selectedEnemy.Attack;
                    texts[3].text = "Speed: " + selectedEnemy.AverageSpeed;
                }
            }
            else
            {
                EnemyPanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("ENEMY PANEL NOT FOUND");
        }
    }

    public void DisplayPlayerScores()
    {
        //Set ScorePanel and score text height according to number of players
        int height = 50 + 100 * PhotonNetwork.PlayerList.Length;

        ScorePanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        Text[] texts = ScorePanel.GetComponentsInChildren<Text>();
        texts[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        string scoreText = "";
        //Retrieve player scores
        foreach (var player in PhotonNetwork.PlayerList)
        {
            //Add player scores to score string
            //(with matching Colour)
            string colString = "#000000";
            string you = "";

            if (player == PhotonNetwork.LocalPlayer) you = " [ YOU ] ";

            if (player.CustomProperties.ContainsKey("colour"))
            {
                //Get player Colour
                if (player.CustomProperties.ContainsKey("colour"))
                {
                    Color col = GameManager.ColourList[(int)player.CustomProperties["colour"]];
                    colString = ColorUtility.ToHtmlStringRGB(col);
                }

            }

            //PRINT TEXT
            scoreText += "<color=#" + colString + ">" + player.NickName + you + ": " + player.CustomProperties["score"] + "</color> \n";
        }

        //Display scores
        texts[1].text = scoreText;

        //TIME
        //Also display time elapsed
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("startTime"))
        {
            double startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["startTime"];
            double timeElapsed = PhotonNetwork.Time - startTime;
            float timeLeft = (float)GameLength - (float)timeElapsed;

            //Time left in seconds
            timeLeft = (Mathf.Round(timeLeft * 100)) / 100;

            //Convert to mins and secs
            int mins = Mathf.FloorToInt(timeLeft / 60);
            int secs = Mathf.FloorToInt(timeLeft % 60);

            if (timeLeft < 0)
            {
                GameOver = true;

                WinPanel.SetActive(true);
                ScorePanel.SetActive(false);

                WinPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height+300);

                Text[] winTexts = WinPanel.GetComponentsInChildren<Text>();

                //Get player with most points
                List<Photon.Realtime.Player> winners = new List<Photon.Realtime.Player>();
                int highScore = 0;

                foreach(var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.ContainsKey("score"))
                    {
                        int currentScore = (int)player.CustomProperties["score"];

                        //If higher than last highest, update winner
                        if (currentScore > highScore)
                        {
                            winners.Clear(); //clear list first, as not currently tie potential
                            winners.Add(player);
                            highScore = currentScore;

                         //If equal to last highest, a tie is possible
                        }
                        else if (currentScore > 0 && currentScore == highScore)
                        {
                            winners.Add(player);
                            highScore = currentScore;
                        }
                    }
                }

                //Draw Header Text
                bool won = false;

                foreach (var winner in winners)
                {

                    //Winners
                    if(winner == PhotonNetwork.LocalPlayer)
                    {
                        won = true;
                        Audio.transform.Find("AudioWin").gameObject.GetComponent<AudioSource>().Play();

                        //Draw
                        if(winners.Count > 1)
                        {
                            winTexts[0].text = "You Won!";
                        }
                        //Solo Winner
                        else
                        {
                            winTexts[0].text = "You Won!";
                        }
                            
                    }
                }

                //Those not among the winner(s)...
                if (won == false)
                {
                    Audio.transform.Find("AudioLose").gameObject.GetComponent<AudioSource>().Play();
                    winTexts[0].text = "You Lost!";
                }

                //Show final scores...
                winTexts[1].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height+300);
                winTexts[1].text = scoreText;

                mins = 0;
                secs = 0;
            }

            texts[2].text = "Time Left: " + mins + ":" + secs.ToString("00");
        }
    }

    private void PrintResources()
    {
        if (CreditText && MetalText && EnergyText)
        {
            CreditText.text = PlayerInventory.Credit.ResourceValue.ToString();
            MetalText.text = PlayerInventory.Metal.ResourceValue.ToString();
            EnergyText.text = EnergyUsed + " / " + PlayerInventory.Energy.ResourceValue.ToString();
        }
    }

    public void DeselectAllPlanets()
    {
        PlayerObject.GetComponent<PlanetSelector>().DeselectAll();
    }

    public void DeselectAllEnemyFormations()
    {
        foreach (Formation form in PlayerObject.GetComponent<FormationSelector>().SelectedObjects)
        {
            if (form.Owner != this)
            {
                PlayerObject.GetComponent<FormationSelector>().DeselectSingle(form);
            }
        }
    }

    public void DeselectAllOwnedFormations()
    {
        foreach (Formation form in PlayerObject.GetComponent<FormationSelector>().SelectedObjects)
        {
            if (form.Owner == this)
            {
                PlayerObject.GetComponent<FormationSelector>().DeselectSingle(form);
            }
        }
    }

    public void DisbandAllFormations()
    {
        FM.DisbandAllFormations();
    }

    public void MakeIntoAttackFormation()
    {
        FM.CreateUnitFormation("TriangleFormation");
    }

    public void MakeIntoDefenseFormation()
    {
        FM.CreateUnitFormation("StraightLineFormation");
    }

    //Increases the player score property (also need to rehash player color)
    public static void IncreaseScore(int amount)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("score"))
        {
            int currentScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["score"];
            int playerColour = (int)PhotonNetwork.LocalPlayer.CustomProperties["colour"];

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("score", currentScore + amount);
            hash.Add("colour", playerColour);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
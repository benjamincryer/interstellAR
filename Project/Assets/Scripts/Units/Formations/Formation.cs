using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public abstract class Formation : MonoBehaviourPun
{
    public PlayerController Owner { get; set; }
    public MathController MathController { get; set; }
    public ResourceFinder RF { get; set; }
    public float MinDistance { get; set; }
    public int Rows { get; set; }
    public Unit CentralUnit { get; set; }
    public List<Unit> ContainedUnits { get; set; }
    public GameObject FormationDestination { get; set; }
    public Vector3 DestinationPosition { get; set; }
    public List<Tuple<Unit, Unit, int, int>> CurrentBattle { get; set; }
    public bool Formed { get; set; }
    public bool Move { get; set; }
    public bool Attack { get; set; }
    public float AttackTime { get; set; }
    public float AttackPeriod { get; set; }
    public Formation FormationToAttack { get; set; }
    public float AttackRange { get; set; }
    public bool Defend { get; set; }
    public float Speed { get; set; }
    public float AverageSpeed { get; set; }
    public int Health { get; set; }
    public string FormationName { get; set; }
    private bool atDestination { get; set; }

    public void Awake()
    {
        MathController = GameObject.Find("GameController").GetComponent<MathController>();

        ContainedUnits = new List<Unit>();
    }

    public void Start()
    {
        Formed = false;
        Move = false;
        atDestination = false;
        Rows = 4;
        AttackTime = 1;
        AttackPeriod = 0;
        AttackRange = 20f;

        InvokeRepeating("CaptureDestinationPlanet", 0, 1f); //Increase capture every second (if at planet)

        RF = GameObject.Find("GameController").GetComponent<ResourceFinder>();
    }

    public void Update()
    {
        RemoveDeadUnits();
        CalculateHealth();
        if (CentralUnit)
        {
            AttackFunction();
            if (FormationDestination == null) { FormationDestination = CentralUnit.gameObject; }
            HighlightCentralUnit();
            UpdateDestinationPosition();
            if (!CheckFormed())
            {
                CalculateFormationPositions();
                CheckFormed();
                MoveToFormation();
                LookAtDestination();
            }
            else
            {
                Vector3 direction = DestinationPosition - gameObject.transform.position;
                if (Move)
                {
                    MoveFormation();
                    RotateFormation(direction);
                }
                else
                {
                    SetFormationRotation();
                }
            }
        }
    }

    public void DestroyFormation()
    {
        photonView.RPC("RPCDestroy", RpcTarget.AllBuffered);
        //Debug.Log("Destroy Empty Formation");
    }

    public abstract void CalculateFormationPositions();

    private void AttackFunction()
    {
        if (Attack)
        {
            if (!CheckFormationToAttackDefeated() && FormationToAttack)
            {
                if (Vector3.Distance(gameObject.transform.position, FormationToAttack.transform.position) >= AttackRange)
                {
                    Move = false;
                    gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, FormationToAttack.transform.position, Speed * Time.deltaTime);
                    gameObject.transform.LookAt(FormationToAttack.transform);
                }
                else
                {
                    //Attacks once every attack time seconds
                    if (Time.time > AttackPeriod)
                    {
                        AttackPeriod = Time.time + AttackTime;
                        AttackFormation();
                        InitiateAttack();
                        Attack = true;
                    }
                }
            }
            else
            {
                FormationToAttack = null;
                if (ContainedUnits.Count == 1)
                {
                    Owner.FM.CreateUnitFormation("SingleUnitFormation", ContainedUnits);
                }
                else if (ContainedUnits.Count > 1)
                {
                    Owner.FM.CreateUnitFormation("StraightLineFormation", ContainedUnits);
                }
            }
        }
    }

    //Sets up generic attributes, i.e. damage, speed and name
    public void SetupFormation()
    {
        CalculateSpeed();
        CalculateAverageSpeed();
        CalculateHealth();
    }

    public void AttackFormation()
    {
        //Generates all attacks for each attacking unit and all defends from each defending unit
        //First unit is attacking unit
        //Second unit is defending unit
        //1st int is attack rating of attacking unit
        //2nd int is defense rating of defending unit
        gameObject.transform.LookAt(FormationToAttack.transform);
        CurrentBattle = new List<Tuple<Unit, Unit, int, int>>();
        foreach (Unit unitFriendly in ContainedUnits)
        {
            int attackRoll = Random.Range(1, unitFriendly.Damage);
            int randomAttackIndex = Random.Range(0, FormationToAttack.ContainedUnits.Count);
            Unit unitToAttack = FormationToAttack.ContainedUnits[randomAttackIndex];
            int defenseRoll = Random.Range(1, unitToAttack.Health);
            CurrentBattle.Add(new Tuple<Unit, Unit, int, int>(unitFriendly, unitToAttack, attackRoll, defenseRoll));
        }
    }

    //If close enough initiates attacks for each tuple in the generated battle sequence
    private void InitiateAttack()
    {
        foreach (Tuple<Unit, Unit, int, int> tpl in CurrentBattle)
        {
            //Create a laser object to travel to the unit
            CreateLaser(tpl);
        }
    }

    //Creates a laser object
    private void CreateLaser(Tuple<Unit, Unit, int, int> tpl)
    {
        var laser = PhotonNetwork.Instantiate("Prefabs/Attacks/LaserBall", tpl.Item1.transform.position, Quaternion.identity);
        laser.GetComponent<Renderer>().material.color = Color.green;

        Laser laserComp = laser.GetComponent<Laser>();
        laserComp.Speed = 50f;
        laserComp.Target = tpl.Item2;
        laserComp.Damage = tpl.Item3;
        laserComp.TargetDefense = tpl.Item4;
    }

    //Moves the entire formation towards the destination of the formation by moving the central unit
    public void MoveFormation()
    {
        if (DestinationPosition != null) gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, DestinationPosition, Speed * Time.deltaTime);
    }

    public void RotateFormation(Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        //Slerp to the desired rotation over time
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rotation, CentralUnit.RotationSpeed * Time.deltaTime);
    }

    //Moves each individual unit in the formation to their designated position, i.e. their destination
    public void MoveToFormation()
    {
        foreach (Unit unit in ContainedUnits)
        {
            if(unit.Destination != null) unit.transform.localPosition = Vector3.MoveTowards(unit.transform.localPosition, unit.Destination, unit.Speed * Time.deltaTime);
        }
    }

    //Checks if a formation is correctly formed, i.e. all units are at their destinations
    public bool CheckFormed()
    {
        foreach (Unit unit in ContainedUnits)
        {
            if (Vector3.Distance(unit.transform.localPosition, unit.Destination) >= 0.2f)
            {
                Formed = false;
                return false;
            }
        }
        Formed = true;

        foreach (Unit unit in ContainedUnits)
        {
            SetUnitDestinationReachedRotation(unit);
        }

        return true;
    }

    //Removes all dead units from the formation
    public void RemoveDeadUnits()
    {
        bool central = false;
        for (int i = 0; i < ContainedUnits.Count; i++)
        {
            if (ContainedUnits[i] != null)
            {
                if (ContainedUnits[i].Health <= 0)
                {
                    if (ContainedUnits[i] == CentralUnit)
                    {
                        central = true;
                    }

                    //Give destroyer points, then destroy object
                    ContainedUnits[i].GivePoints(20);
                    ContainedUnits[i].DestroyUnit();
                    photonView.RPC("RPCRemoveUnitAt", RpcTarget.AllBuffered, i as object);
                }
            }
            else
            {
                photonView.RPC("RPCRemoveUnitAt", RpcTarget.AllBuffered, i as object);
            }
        }
        //If the central unit dies, the formation needs to reform
        if (central)
        {
            //Calculate a new central unit
            CentralUnit = MathController.CalculateCentralUnit(ContainedUnits);
            CalculateFormationPositions();
            MoveToFormation();
        }
        CalculateName();
    }

    //Checks if the formation this formation is attacking is defeated, i.e. has no units left
    public bool CheckFormationToAttackDefeated()
    {
        if (FormationToAttack)
        {
            if (FormationToAttack.ContainedUnits.Count <= 0)
            {
                CentralUnit = MathController.CalculateCentralUnit(ContainedUnits);
                return true;
            }
        }
        return false;
    }

    /*Updates the position to which the formation will travel, so that the formation for example will appear above its destination
      rather than inside it*/

    public void UpdateDestinationPosition()
    {
        if (FormationDestination != null)
        {
            var pos = FormationDestination.transform.position;
            float radius = MathController.CalculateRadiusSphere(FormationDestination);
            DestinationPosition = new Vector3(pos.x, pos.y + radius + 2, pos.z);
        }
    }

    //ROTATION FUNCTIONS
    //Sets the rotation of all units to  the generic quaternion identity rotation
    public void SetQuaternionRotation()
    {
        foreach (Unit unit in ContainedUnits)
        {
            unit.transform.rotation = Quaternion.identity;
        }
    }

    //Sets the rotation of all units to the rotation of the central, and sets the central to look at the formation destination
    public void SetCentralRotation()
    {
        foreach (Unit unit in ContainedUnits)
        {
            if (unit == CentralUnit)
            {
                unit.transform.LookAt(FormationDestination.transform);
            }
            else
            {
                unit.transform.rotation = CentralUnit.transform.rotation;
            }
        }
    }

    //Sets the rotation of all units to the rotation of the overall formation destination object rotation
    public void SetDestinationRotation()
    {
        foreach (Unit unit in ContainedUnits)
        {
            unit.transform.rotation = FormationDestination.transform.rotation;
        }
    }

    //Sets the rotation of all to the rotation of the formation gameobject
    public void SetFormationRotation()
    {
        foreach (Unit unit in ContainedUnits)
        {
            unit.transform.rotation = gameObject.transform.rotation;
        }
    }

    //Makes all units look at their own individual destinations
    public void LookAtDestination()
    {
        foreach (Unit unit in ContainedUnits)
        {
            unit.transform.LookAt(unit.Destination);
        }
    }

    //Makes all units look at the destination of the overall formation
    public void LookAtFormationDestination()
    {
        foreach (Unit unit in ContainedUnits)
        {
            if (FormationDestination)
            {
                unit.transform.LookAt(FormationDestination.transform);
            }
        }
    }

    //Checks if a unit has reached its destination, and sets its rotation accordingly
    public bool SetUnitDestinationReachedRotation(Unit unit)
    {
        if (Vector3.Distance(unit.transform.position, unit.Destination) <= 0.5f)
        {
            unit.transform.rotation = Quaternion.identity;
            return true;
        }
        return false;
    }

    public void CaptureDestinationPlanet()
    {
        //Check if Central unit is at its destination
        if (Vector3.Distance(CentralUnit.transform.position, DestinationPosition) <= 0.5f)
        {
            //Engine->Idle sound
            var moveSound = transform.GetChild(0).gameObject;
            var idleSound = transform.GetChild(1).gameObject;

            moveSound.GetComponent<AudioSource>().Stop();
            idleSound.GetComponent<AudioSource>().Play();

            atDestination = true;
        }
        else
        {
            atDestination = false;
        }
            

        //If at destination
        if (atDestination)
        {
            var pl = FormationDestination.gameObject.GetComponent<Planet>();

            //Check if destination is a Planet
            if (pl)
            {
                //Capture rate scales with Formation size
                float rate = 0.5f * ContainedUnits.Count;

                var view = FormationDestination.GetPhotonView();

                //Set Capture ID of planet
                view.RPC("SetCaptureID", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber as object);

                //Transfer ownership to us if the planet has never been touched
                if (pl.CapturePoints == 0)
                    pl.gameObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);

                //If it belongs to us:
                if (view.Owner == PhotonNetwork.LocalPlayer && pl.CapturePoints < 100) //check if its view belongs to master, and then that master doesn't actually own the planet ingame (confusing)
                {
                    //Increase its capture %
                    view.RPC("ChangeCapturePoints", RpcTarget.AllBuffered, rate as object);
                    
                }
                //Else If it belongs to another player: Decrease its capture %
                else if (view.Owner != PhotonNetwork.LocalPlayer && pl.CapturePoints != 0)
                {
                    view.RPC("ChangeCapturePoints", RpcTarget.AllBuffered, -rate as object);
                }

                //Else: Do nothing, as it already belongs to us
            }
        }
    }

    //ATTRIBUTE SETTING FUNCTIONS
    //Calculates the slowest unit speed by which the formation has to move
    public void CalculateSpeed()
    {
        float slowestSpeed = 100f;
        foreach (Unit unit in ContainedUnits)
        {
            if (unit.Speed < slowestSpeed)
            {
                slowestSpeed = unit.Speed;
            }
        }
        Speed = slowestSpeed;
    }

    public void CalculateAverageSpeed()
    {
        float avgSpeed = 0;
        foreach (Unit unit in ContainedUnits)
        {
            avgSpeed += unit.Speed;
        }
        AverageSpeed = avgSpeed / ContainedUnits.Count;
    }

    public void CalculateHealth()
    {
        int health = 0;
        foreach (Unit unit in ContainedUnits)
        {
            health += unit.Health;
        }
        Health = health;
    }

    //Calculates the name of the formation
    public void CalculateName()
    {
        FormationName = "" + name.ToString() + "(" + ContainedUnits.Count + ")";
    }

    //Makes the assigned central unit highlight red
    public void HighlightCentralUnit()
    {
        foreach (Unit unit in ContainedUnits)
        {
            var parts = unit.GetComponentsInChildren<Renderer>();
            if (unit == CentralUnit)
            {
                foreach (Renderer part in parts)
                {
                    //DEBUG - Uncomment to highlight the central unit
                    //part.material.shader = RF.selectShader;
                    //part.material.SetColor("_FirstOutlineColor", Color.yellow);
                }
            }
            else
            {
                foreach (Renderer part in parts)
                {
                    Shader.Find("Standard");
                }
            }
        }
    }

}
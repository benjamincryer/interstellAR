using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FormationManager : MonoBehaviourPun
{
    public List<Formation> ControlledFormations { get; set; }
    public ResourceFinder ResourceFinder { get; set; }
    public MathController MathController { get; set; }
    public PlayerController Player { get; set; }

    public void Update()
    {
        DestroyEmptyFormations();
    }

    public void Start()
    {
        ControlledFormations = new List<Formation>();
        ResourceFinder = GameObject.Find("GameController").GetComponent<ResourceFinder>();
        MathController = GameObject.Find("GameController").GetComponent<MathController>();
    }

    //Destroys all formation objects with no units
    private void DestroyEmptyFormations()
    {
        /*
        for (int i = 0; i < ControlledFormations.Count; i++)
        {
            
            if (ControlledFormations[i] != null)
            {
                if (ControlledFormations[i].gameObject.transform.childCount == 0 || ControlledFormations[i].ContainedUnits.Count <= 0)
                {
                    GameObject reference = ControlledFormations[i].gameObject;
                    Player.PlayerObject.GetComponent<FormationSelector>().SelectedObjects.Remove(ControlledFormations[i]);
                    //Destroy(ControlledFormations[i]);
                    ControlledFormations.RemoveAt(i);

                    reference.GetComponent<Formation>().DestroyFormation();
                }
            }
            else
            {
                Player.PlayerObject.GetComponent<FormationSelector>().SelectedObjects.Remove(ControlledFormations[i]);
                ControlledFormations.RemoveAt(i);
            }
        }*/

        for (int i = 0; i < ControlledFormations.Count; i++)
        {
            if (ControlledFormations[i] == null)
            {
                Player.PlayerObject.GetComponent<FormationSelector>().SelectedObjects.Remove(ControlledFormations[i]);
                ControlledFormations.RemoveAt(i);
            }
        }

        //Destroy all empty formations owned
        var forms = GameObject.FindObjectsOfType<Formation>();
        for (int i = 0; i < forms.Length; i++)
        {
            if (forms[i].gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
            {
                if (forms[i].gameObject.transform.childCount < 3 || forms[i].ContainedUnits.Count <= 0)
                {
                    forms[i].DestroyFormation();
                }
            }

        }
    }


    //Creates a formation of formation name by finding all selected formations and combining them
    public int CreateUnitFormation(string formationName)
    {
        GameObject formationObject = FormationFactory(formationName);
        List<Formation> ownedFormations = GetOwnedFormations(Player.SelectedFormations);
        if (ownedFormations.Count > 0)
        {
            List<Unit> units = GetUnitsInOwnedFormations(ownedFormations);
            Unit central = MathController.CalculateCentralUnit(units);
            if (central)
            {
                formationObject.transform.position = central.transform.position;
            }
            formationObject.GetComponent<Formation>().Owner = Player;
            SetUpFormation(formationObject.GetComponent<Formation>(), central, units, 5f);
            SetParents(units, formationObject);
            Player.PlayerObject.GetComponent<FormationSelector>().DeselectAll();
            //Player.PlayerObject.GetComponent<FormationSelector>().SelectSingle(formationObject.GetComponent<Formation>());
            ControlledFormations.Add(formationObject.GetComponent<Formation>());

            return formationObject.GetPhotonView().ViewID;
        }
        return -1;
    }

    //Creates a formation of with the list of units passed in
    public int CreateUnitFormation(string formationName, List<Unit> units)
    {
        if(units.Count > 0)
        {
            GameObject formationObject = FormationFactory(formationName);
            Unit central = MathController.CalculateCentralUnit(units);
            if (central)
            {
                formationObject.transform.position = central.transform.position;
            }
            formationObject.GetComponent<Formation>().Owner = Player;
            SetUpFormation(formationObject.GetComponent<Formation>(), central, units, 5f);
            SetParents(units, formationObject);
            Player.PlayerObject.GetComponent<FormationSelector>().DeselectAll();
            //Player.PlayerObject.GetComponent<FormationSelector>().SelectSingle(formationObject.GetComponent<Formation>());
            ControlledFormations.Add(formationObject.GetComponent<Formation>());

            return formationObject.GetPhotonView().ViewID;
        }
        return -1;
    }

    //Creates a formation that organises units into the type of formation specified
    public GameObject FormationFactory(string formationName)
    {
        //Creates a formation for multiple units ordered to move
        GameObject formationObject;

        formationObject = PhotonNetwork.Instantiate("Prefabs/Formations/" + formationName, new Vector3(0, 0, 0), Quaternion.identity);
        formationObject.GetPhotonView().RPC("RPCSetParent",RpcTarget.AllBuffered);

        return formationObject;
    }

    //Sets the central unit of a formation and also adds the selected units to formation
    public void SetUpFormation(Formation formation, Unit central, List<Unit> allUnits, float minDistance)
    {
        if(allUnits.Count > 0)
        {
            //Ensure that the central unit is always the first in the contained units
            formation.CentralUnit = central;
            
            //formation.ContainedUnits.Add(formation.CentralUnit);
            formation.gameObject.GetPhotonView().RPC("RPCAddUnit", RpcTarget.AllBuffered, formation.CentralUnit.gameObject.GetPhotonView().ViewID as object);

            foreach (Unit unit in allUnits)
            {
                if (unit != central)
                {
                    //formation.ContainedUnits.Add(unit);
                    formation.gameObject.GetPhotonView().RPC("RPCAddUnit", RpcTarget.AllBuffered, unit.gameObject.GetPhotonView().ViewID as object);
                }
            }
            formation.SetupFormation();
            formation.MinDistance = minDistance;
        }
    }

    //Returns all unit components in a formation
    private List<Unit> GetUnitsInFormation(Formation form)
    {
        Unit[] unitArray = form.GetComponentsInChildren<Unit>();
        List<Unit> units = new List<Unit>();
        for (int i = 0; i < unitArray.Length; i++)
        {
            units.Add(unitArray[i]);
        }
        return units;
    }

    //Disbands all selected formations into single unit formations
    public void DisbandAllFormations()
    {
        foreach (Formation formation in Player.PlayerObject.GetComponent<FormationSelector>().SelectedObjects)
        {
            //Only disband your own formations
            if (formation.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
            {
                Player.PlayerObject.GetComponent<FormationSelector>().ResetShaderAll();
                Formation temp = formation;
                foreach (Unit unit in formation.ContainedUnits)
                {
                    List<Unit> unitList = new List<Unit>();
                    unitList.Add(unit);
                    CreateUnitFormation("SingleUnitFormation", unitList);
                }
            }
        }
        Player.PlayerObject.GetComponent<FormationSelector>().SelectAll();
    }

    //Returns all units in all selected formations (that are owned by the player)
    private List<Unit> GetUnitsInOwnedFormations(List<Formation> formations)
    {
        List<Unit> units = new List<Unit>();
        foreach (Formation form in formations)
        {
            List<Unit> unitsInFormation = GetUnitsInFormation(form);
            foreach (Unit unit in unitsInFormation)
            {
                if (unit.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
                {
                    units.Add(unit);
                }
            }
        }
        return units;
    }

    //Sets the parents of the formation and its units
    private void SetParents(List<Unit> units, GameObject formation)
    {
        //formation.transform.parent = ResourceFinder.unitsParent.transform;
        foreach (Unit unit in units)
        {
            unit.SetParent(formation.GetPhotonView().ViewID);
        }
    }

    //Checks through the formations and returns all formations owned by the player
    private List<Formation> GetOwnedFormations(List<Formation> formations)
    {
        List<Formation> ownedFormations = new List<Formation>();
        foreach (Formation form in formations)
        {
            if (form.Owner == Player)
            {
                ownedFormations.Add(form);
            }
        }
        return formations;
    }
}
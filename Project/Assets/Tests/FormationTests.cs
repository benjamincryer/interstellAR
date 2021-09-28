using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class FormationTests : MonoBehaviour
{
    private int numberOfUnitIterations = 2;
    private List<Formation> formations;

    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("0_LOAD", LoadSceneMode.Single);
        GameObject.Find("INSERT SCENE NAME HERE").GetComponent<NetworkConnectionManager>().sceneLoad = "FormationScene";
        formations = new List<Formation>();
    }

    [UnityTest]
    [Description("Tests that units are assigned to the single unit formation upon creation")]
    public IEnumerator CorrectAssignmentToSingleUnitFormation()
    {
        for (int i = 0; i < numberOfUnitIterations; i++)
        {
            GameObject.Find("GameController").GetComponent<PlayerController>().CreateUnit("AttackFighter");
            GameObject.Find("GameController").GetComponent<PlayerController>().CreateUnit("BattleCruiser");
            GameObject.Find("GameController").GetComponent<PlayerController>().CreateUnit("Destroyer");
        }
        Assert.AreEqual(numberOfUnitIterations * 3, GameObject.Find("GameController").GetComponent<PlayerController>().FM.ControlledFormations.Count);
        foreach (Formation f in GameObject.Find("GameController").GetComponent<PlayerController>().FM.ControlledFormations)
        {
            Assert.IsInstanceOf(typeof(SingleUnitFormation), f);
            formations.Add(f);
        }
        yield return null;
    }

    [UnityTest]
    [Description("Tests that units are correctly assigned to a straight line formation and its object")]
    public IEnumerator CorrectAssignmentToStraightLineFormation()
    {
        GameObject.Find("GameController").GetComponent<PlayerController>().FM.CreateUnitFormation("StraightLineFormation");
        foreach (Formation f in GameObject.Find("GameController").GetComponent<PlayerController>().FM.ControlledFormations)
        {
            Assert.IsInstanceOf(typeof(StraightLineFormation), f);
        }
        yield return null;
    }

    [UnityTest]
    [Description("Tests that units are correctly assigned to a triangle formation and its object")]
    public IEnumerator CorrectAssignmentToTriangleFormation()
    {
        GameObject.Find("GameController").GetComponent<PlayerController>().FM.CreateUnitFormation("TriangleFormation");
        foreach (Formation f in GameObject.Find("GameController").GetComponent<PlayerController>().FM.ControlledFormations)
        {
            Assert.IsInstanceOf(typeof(TriangleFormation), f);
        }
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
    }
}
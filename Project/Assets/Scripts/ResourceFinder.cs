using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used to make the process of finding resources from scripts easier, if necessary
public class ResourceFinder : MonoBehaviour
{
    public Shader selectShader;
    public GameObject planetsParent;
    public GameObject unitsParent;
    public Text planetSelectedText;
    public Text formationSelectedText;
    public GameObject explosionPrefab;
    public Shader centralShader;
    public Text playerResourceText;
    public GameObject selectedFormationTextPrefab;
}
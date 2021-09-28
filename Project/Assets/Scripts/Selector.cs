using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Abstract generic class to create object selectors
public abstract class Selector<T> : MonoBehaviour where T : MonoBehaviour
{
    public Shader selectShader;
    public Text selectedText;

    public T SelectedObject { get; set; }
    public List<T> SelectedObjects { get; set; }
    public T[] InstancedObjects { get; set; }

    private void Awake()
    {
        SelectedObjects = new List<T>();
    }

    public void Update()
    {
        Select();
        PrintInformation();
    }

    public void Select()
    {
        #if UNITY_ANDROID
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    //Has to be a single tap
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            InstancedObjects = FindObjectsOfType<T>();
                            if (typeof(T) == typeof(Formation))
                            {
                                HardChildSelection(hit);
                            }
                            else
                            {
                                SelectDirectObject(hit);
                            }
                        }
                    }
                }

        #else

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                InstancedObjects = FindObjectsOfType<T>();
                if (typeof(T) == typeof(Formation))
                {
                    HardChildSelection(hit);
                }
                else
                {
                    SelectDirectObject(hit);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //Right click deselects - this is because buttons will cause deselection, and consequently not allow for e.g. buildings
            DeselectAll();
        }

        #endif
    }

    private void SelectDirectObject(RaycastHit hit)
    {
        //Loops through all instances of the objects we're trying to select
        foreach (T obj in InstancedObjects)
        {
            //If the raycast hit any of them:
            if (obj.transform == hit.transform)
            {
                //(Only select 1 planet max)
                if (CheckSelectedPlanetLimit())
                {
                    DeselectAll();
                }
                SelectedObject = obj;
                if (!CheckDuplicate(SelectedObject))
                {
                    SelectedObjects.Add(SelectedObject);
                    //Highlight all object parts (for objects composed of multiple child objects)
                    var parts = SelectedObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer part in parts)
                    {
                        part.material.shader = selectShader;
                        part.material.SetColor("_FirstOutlineColor", Color.yellow);
                    }
                }
            }
        }
    }

    private Boolean CheckSelectedPlanetLimit()
    {
        if (typeof(T).Equals(typeof(Planet)) && SelectedObjects.Count == 1)
            return true;
        return false;
    }

    //If an enemy formation is already selected, then return that object. Else null
    public T CheckSelectedEnemyLimit()
    {
        if (typeof(T).Equals(typeof(Formation)))
        {
            foreach (T obj in SelectedObjects)
            {
                if (obj.gameObject.GetPhotonView().Owner != PhotonNetwork.LocalPlayer)
                    return obj;
            }
        }
        return null;
    }

    private Boolean CheckDuplicate(T hitObject)
    {
        //Checks if the selected object hasn't already been selected
        for (int i = 0; i < SelectedObjects.Count; i++)
        {
            if (SelectedObjects[i] == hitObject)
            {
                //Return true if duplicate found
                return true;
            }
        }
        return false;
    }

    //Selects children graphically only
    private void WeakChildSelection(Transform selection, RaycastHit hit)
    {
        foreach (Transform child in selection.transform)
        {
            if (child == hit.transform)
            {
                //Highlight all object parts (for objects composed of multiple child objects)
                var parts = child.GetComponentsInChildren<Renderer>();
                foreach (Renderer obj in parts)
                {
                    obj.material.shader = selectShader;
                    obj.material.SetColor("_FirstOutlineColor", Color.yellow);
                }
            }
        }
    }

    //Adds object if any child is selected;
    private void HardChildSelection(RaycastHit hit)
    {
        //Loops through all instances of the objects we're trying to select (Formation in this case)
        foreach (T obj in InstancedObjects)
        {
            //If the raycast hit any of the children (ie. a Unit)
            foreach (Transform child in obj.transform)
            {
                if (child == hit.transform)
                {
                    if (!CheckDuplicate(obj))
                    {
                        if (child.gameObject.GetPhotonView().Owner == PhotonNetwork.LocalPlayer || 
                            (child.gameObject.GetPhotonView().Owner != PhotonNetwork.LocalPlayer && CheckSelectedEnemyLimit() == null))
                        {
                            //Select the Formation
                            SelectedObjects.Add(obj);

                            //Highlight all object parts (for objects composed of multiple child objects)
                            var parts = obj.GetComponentsInChildren<Renderer>();
                            foreach (Renderer part in parts)
                            {
                                part.material.shader = selectShader;
                                part.material.SetColor("_FirstOutlineColor", Color.yellow);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SelectAll()
    {
        InstancedObjects = FindObjectsOfType<T>();
        //If left mouse button is clicked
        foreach (T obj in InstancedObjects)
        {
            if (!CheckDuplicate(obj))
            {
                SelectedObject = obj;
                SelectedObjects.Add(SelectedObject);
                //Highlight all object parts (for objects composed of multiple child objects)
                var parts = SelectedObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer part in parts)
                {
                    part.material.shader = selectShader;
                    part.material.SetColor("_FirstOutlineColor", Color.yellow);
                }
            }
        }
        PrintInformation();
    }

    public void DeselectAll()
    {
        foreach (T obj in SelectedObjects)
        {
            if (obj != null)
            {
                if (obj.GetComponent<Renderer>())
                {
                    Renderer part = obj.GetComponent<Renderer>();
                    part.material.shader = Shader.Find("Standard");
                }
                var parts = obj.GetComponentsInChildren<Renderer>();
                //Set shader to standard to remove highlighted shader

                foreach (Renderer part in parts)
                {
                    part.material.shader = Shader.Find("Standard");
                }
            }
        }
        SelectedObjects.Clear();
        PrintInformation();
    }

    public void DeselectSingle(T obj)
    {
        foreach (T selected in SelectedObjects)
        {
            if (selected == obj)
            {
                if (obj.GetComponent<Renderer>())
                {
                    Renderer part = obj.GetComponent<Renderer>();
                    part.material.shader = Shader.Find("Standard");
                }
                var parts = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer part in parts)
                {
                    part.material.shader = Shader.Find("Standard");
                }
                //Set shader to standard to remove highlighted shader
                SelectedObjects.Remove(selected);
                break;
            }
        }
        PrintInformation();
    }

    public void SelectSingle(T obj)
    {
        if (!CheckDuplicate(obj))
        {
            SelectedObject = obj;
            SelectedObjects.Add(SelectedObject);
            //Highlight all object parts (for objects composed of multiple child objects)
            var parts = SelectedObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer part in parts)
            {
                part.material.shader = selectShader;

                //If enemy, highlight Red
                if (part.gameObject.GetPhotonView().Owner != PhotonNetwork.LocalPlayer)
                {
                    part.material.SetColor("_FirstOutlineColor", Color.yellow);
                }
                else
                {
                    //Else Friend
                    part.material.SetColor("_FirstOutlineColor", Color.yellow);
                }
            }
        }
    }

    public void SelectObjectHightlightChildren(T obj)
    {
        foreach (Transform child in obj.transform)
        {
            SelectedObjects.Add(obj);
            //Highlight all object parts (for objects composed of multiple child objects)
            var parts = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer part in parts)
            {
                part.material.shader = selectShader;
                part.material.SetColor("_FirstOutlineColor", Color.yellow);
            }
        }
    }

    public void ResetShaderAll()
    {
        foreach (T t in SelectedObjects)
        {
            var parts = t.GetComponentsInChildren<Renderer>();
            foreach (Renderer part in parts)
            {
                part.material.shader = Shader.Find("Standard");
            }
        }
    }

    public abstract void PrintInformation();
}
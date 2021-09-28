using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sun : HeavenlyObject
{
    public int sunEnergy = 10000;

    void Awake()
    {
        Mass = sunEnergy;
        RotationSpeed = 1 / transform.localScale.magnitude;
        OrbitObject = gameObject;
        OrbitSpeed = orbitConstant;
        transform.parent = GameObject.Find("Planets").transform;
    }

    void Update()
    {
        transform.Rotate(0, Time.deltaTime * RotationSpeed, 0);
        Orbit();
    }
}

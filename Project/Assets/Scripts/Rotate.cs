using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    //Speed in each direction
    public float xSpeed, ySpeed, zSpeed;

    void Update()
    {
        transform.Rotate(Time.deltaTime * xSpeed, Time.deltaTime * ySpeed, Time.deltaTime * zSpeed, Space.Self);
    }
}
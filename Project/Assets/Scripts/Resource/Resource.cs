using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float ResourceValue { get; set; }
    public string ResourceName { get; set; }
    public float ResourceRate { get; set; }

    public void SetupResource(string resourceName, float initialValue, float initialResourceRate)
    {
        ResourceName = resourceName;
        ResourceValue = initialValue;
        ResourceRate = initialResourceRate;
    }

    public void Update()
    {
        if (ResourceValue < 0)
        {
            ResourceValue = 0;
        }
    }
}
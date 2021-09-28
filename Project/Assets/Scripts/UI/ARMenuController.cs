using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //AR mode on Android, Freecam mode on desktop
#if UNITY_ANDROID
        GetComponent<MainCameraMovement>().enabled = false;
        GetComponent<GyroCamera>().enabled = true;
#endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{

    public static float latitude, longitude, altitude;
    public Text coordinates;
    public GameObject ObjectParent;
    
    private IEnumerator coroutine;
    private LocationInfo prevLoc;
    private const float GPS_TIME = 1f;
    private const int MAX_WAIT = 20;

    private void Start()
    {
        //Persistent object (between Scenes)
        //DontDestroyOnLoad(gameObject);

        Input.compass.enabled = true;

        StartCoroutine(StartLocationService());
    }

    //A thread that attempts to start the Location service for a set amount of time before giving up
    private IEnumerator StartLocationService()
    {
        //Check if GPS is enabled on user's device
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        //Start GPS service
        Input.location.Start();

        //Keep waiting until GPS status is Initializing
        int maxWait = MAX_WAIT;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        //Timeout after 20 seconds
        if (maxWait <= 0)
        {
            Debug.Log("GPS timed out");
            yield break;
        }

        //Check if GPS service failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS failed to determine location");
        }
        else
        {
            GPSInit();
        }
    }

    //GPS initialization method, called once the GPS service has successfully started and is in the Running state
    private void GPSInit()
    {
        //Get GPS coordinates
        GetGPS();
        prevLoc = Input.location.lastData;

        //Update gameobject positions
        UpdateGPSObjects();

        //Now that objects are generated, enable Gyroscope and rotate camera to align Unity y axis with North
        Camera.main.GetComponent<GyroCamera>().enabled = true;

        //Start Update coroutine
        coroutine = GPSUpdate();
        StartCoroutine(coroutine);
    }

    //A thread that updates the player's GPS coordinates at a constant rate
    private IEnumerator GPSUpdate()
    {
        WaitForSeconds updateTime = new WaitForSeconds(GPS_TIME);

        //Loops once every GPS_TIME seconds until coroutine is killed
        while (true)
        {
            //Get current GPS position
            GetGPS();

            //If player's GPS position changed:
            if (prevLoc.latitude != latitude || prevLoc.longitude != longitude)
            {
                //Update gameobject positions
                UpdateGPSObjects();

                //Update North, as the relative angle changes when you move
                //Camera.main.GetComponent<GyroCamera>().UpdateNorth();
            }

            yield return updateTime;
        }
    }

    //Get current GPS position from Location Services
    private void GetGPS()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitude = Input.location.lastData.altitude;
    }

    //Update positions of all GPSObject instances
    private void UpdateGPSObjects()
    {
        foreach (Transform child in ObjectParent.transform)
        {
            child.gameObject.GetComponent<GPSObject>().UpdateLocation();
        }
    }

    //Stop GPS service and Update coroutine
    private void stopGPS()
    {
        Debug.Log("GPS service ended");

        Input.location.Stop();
        StopCoroutine(coroutine);
    }

    //Called when the GameObject this is attached to is disabled/destroyed
    private void OnDisable()
    {
        stopGPS();
    }

    private void Update()
    {
        if (coordinates != null)
        coordinates.text = "LAT: " + latitude.ToString() + "\nLON: " + longitude.ToString() + "\nALT: " + altitude.ToString() + "\nNORTH: " + Input.compass.trueHeading;
    }
}
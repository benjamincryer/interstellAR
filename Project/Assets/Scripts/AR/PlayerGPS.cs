using UnityEngine;
using Photon.Pun;

public class PlayerGPS : GPSObject, IPunObservable
{

    private void Start()
    {
        //Disable renderer for your own Player position (only want to see others)
        if (photonView.IsMine)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    private void Update()
    {
        //Only update your own Player position
        if (photonView.IsMine)
        {
            //Update position
            SetGPS(GPS.latitude, GPS.longitude, GPS.altitude);
            transform.position = Camera.main.transform.position;
        }
    }
}

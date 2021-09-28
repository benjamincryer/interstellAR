using Photon.Pun;
using UnityEngine;

public class GPSObject : MonoBehaviourPun, IPunObservable
{
    public float latitude, longitude, altitude;
    public Vector3 positionOrigin; //the origin Vector3 coordinates of the GPS object (client-specific, based on difference between object GPS and player GPS)
    public Vector3 positionOffset = new Vector3(0,0,0); //the distance moved from its origin point (universal, applied by all clients)

    private float scale = 10000f; //determines the scale of object distances from the player in the game world

    private void Update()
    {
        UpdateOffset();
    }

    //CONTINUOUSLY SEND OR RECEIVE GPS VARIABLES OVER NETWORK 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Writing data to other players (the object's owner is doing this)
            stream.SendNext(latitude);
            stream.SendNext(longitude);
            stream.SendNext(altitude);
            stream.SendNext(positionOffset);
            stream.SendNext(transform.localRotation);
            stream.SendNext(transform.localScale);
        }
        else
        {
            //Receiving data from owner of object
            this.latitude = (float)stream.ReceiveNext();
            this.longitude = (float)stream.ReceiveNext();
            this.altitude = (float)stream.ReceiveNext();
            this.positionOffset = (Vector3)stream.ReceiveNext();
            this.transform.localRotation = (Quaternion)stream.ReceiveNext();
            this.transform.localScale = (Vector3)stream.ReceiveNext();

            //Tell client to update this object's Unity coords based on the origin GPS and the offset coords received
            UpdateLocation();
        }
    }

    //SET TRANSFORM POSITION RELATIVE TO GPS POSITION
    public void UpdateLocation()
    {
        //Convert difference in GPS coordinates to ingame meters
        float zz = (latitude - GPS.latitude) * scale;
        float xx = (longitude - GPS.longitude) * scale;

        //Move object to correct position (player position - GPS origin + positionOffset)
        positionOrigin = new Vector3(xx, altitude, zz);
        Vector3 position = positionOrigin + positionOffset;
        
        transform.position = position;
    }

    //IF THE TRANSFORM CHANGED, UPDATE VECTOR3 OFFSET VALUE
    public void UpdateOffset()
    {
        //If Player Client owns this object, OR the object belongs to the Scene (in which case the Master Client handles this)
        if ((photonView.IsSceneView && PhotonNetwork.IsMasterClient) || photonView.IsMine)
        {
            //Update positionOffset so that other clients receive new position
            positionOffset = transform.position - positionOrigin;
        }
    }

    //Converts a Unity Vector3 coord to GPS (using an origin GPS point, ie. the player location)
    //This will be done when the player's GPS position changes, but before it is updated
    public void Unity2GPS()
    {
        //Convert Unity coords back to GPS, using GPS position of player
        longitude = (transform.position.z / scale) + GPS.longitude;
        latitude = (transform.position.x / scale) + GPS.latitude;

        //longitude = MetersToLong(transform.position.x, GPS.longitude, GPS.latitude);
        //latitude = MetersToLat(transform.position.y, GPS.latitude);

        altitude = transform.position.y;
    }

    public void SetGPS(float latitude, float longitude, float altitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
    }

    /*
    //Converts GPS coordinates to Meters from an origin point
    private float GPSToMeters(float lat1, float lon1, float lat2, float lon2)
    {
        //Radius of earth in KM
        var R = 6378.137;

        var dLat = (lat2 - lat1) * Mathf.PI / 180;
        var dLon = (lon2 - lon1) * Mathf.PI / 180;

        var a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
            Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
            Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var d = R * c;

        return (float) (d * 1000); //Convert to meters
    }

    //Converts y value in meters to Latitude
    private float MetersToLat(float dN, float pLat)
    {
        //Radius of earth in M
        var R = 6378137;

        //Coordinate offset in radians
        var dLat = dN / R;

        //New latitude position with offset added
        var latO = pLat + dLat * 180 / Mathf.PI;

        return latO;
    }

    //Converts x value in meters to Longitude
    private float MetersToLong(float dE, float pLong, float pLat)
    {
        //Radius of earth in M
        var R = 6378137;

        //Coordinate offset in radians
        var dLon = dE / (R * Mathf.Cos(Mathf.PI * pLat / 180));

        //New longitude position with offset added
        var longO = pLong + dLon * 180 / Mathf.PI;

        return longO;
    }
    */

}
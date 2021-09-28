using UnityEngine;
using Photon.Pun;

public class HeavenlyObject : MonoBehaviourPun
{
    public Galaxy galaxy;
    public static float rotationConstant = 1000f, orbitConstant = 1.5f;
    public static float gravity = 9.81f;

    public float Mass { get; set; }
    public float RotationSpeed { get; set; }
    public float OrbitSpeed { get; set; }
    public GameObject OrbitObject { get; set; }
    protected MathController MathController { get; set; }

    public void Orbit()
    {
        if(OrbitObject)
        {
            Vector3 center = OrbitObject.transform.position;
            //Use RotateAround to control the orbit
            transform.RotateAround(center, Vector3.up, OrbitSpeed * Time.deltaTime);
        }
    }
}

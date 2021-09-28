using System.Collections;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    //STATE
    private float initialYAngle = 0f;

    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;
    private Transform rawGyroRotation;
    private float tempSmoothing;

    //SETTINGS
    [SerializeField] private float smoothing = 0.1f;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android) enabled = true; else enabled = false;
    }

    private IEnumerator Start()
    {
        Input.gyro.enabled = true;

        if (Input.compass.enabled == false) Input.compass.enabled = true;

        //Create child object to store the gyroscope's raw input
        rawGyroRotation = new GameObject("GyroRaw").transform;
        rawGyroRotation.parent = transform;
        rawGyroRotation.position = transform.position;
        rawGyroRotation.rotation = transform.rotation;

        //Wait until gyro is active, then calibrate to reset starting rotation
        yield return new WaitForSeconds(1);

        initialYAngle = Input.compass.trueHeading;
        StartCoroutine(CalibrateYAngle());
    }

    private void Update()
    {
        UpdateNorth();

        ApplyGyroRotation();
        ApplyCalibration();

        //Using Slerp for smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rawGyroRotation.rotation, smoothing);
    }

    //A thread that continuously translates the relative gyro angle into real-world positioning
    private IEnumerator CalibrateYAngle()
    {
        tempSmoothing = smoothing;
        smoothing = 1;
        calibrationYAngle = appliedGyroYAngle - initialYAngle; //Offsets the y angle in case it wasn't 0 at edit time
        yield return null;
        smoothing = tempSmoothing;
    }

    //Fetches the gyro attitude and translates it into
    private void ApplyGyroRotation()
    {
        rawGyroRotation.rotation = Input.gyro.attitude;
        rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self); //Swap "handedness" of quaternion from gyro
        rawGyroRotation.Rotate(90f, 180f, 0f, Space.World); //Rotate to make sense as a camera pointing out the back of your device
        appliedGyroYAngle = rawGyroRotation.eulerAngles.y; //Save the angle around y axis for use in calibration
    }

    //Rotates y angle back however much it deviated when calibrationYAngle was saved
    private void ApplyCalibration()
    {
        rawGyroRotation.Rotate(0f, -calibrationYAngle, 0f, Space.World);
    }

    //Updates the initialYAngle with the new direction of North
    public void UpdateNorth()
    {
        initialYAngle = Input.compass.trueHeading;
    }
}
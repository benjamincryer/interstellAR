using UnityEngine;

public class LowPassFilterTransform : MonoBehaviour
{
    private Quaternion currentRotation, targetRotation;
    private Vector3 currentPosition, targetPosition;

    // with these fields you can select whether to apply low pass on rotation or position only
    public bool applyOnRotation = true;

    public bool applyOnPosition = true;

    // This field controls the low pass value
    // Use 1 for no filtering, and a value closer to zero for more sluggish filtering
    // (Note that zero would be invalid and freeze the transform)
    //
    [Range(0.1f, 1.0f)]
    public float lowPassFilter = 0.5f;

    public float LowPassFilter { set { lowPassFilter = value; } }

    // Use this for initialization
    private void Start()
    {
        currentRotation = targetRotation = transform.rotation;
        currentPosition = targetPosition = transform.position;
    }

    // Use LateUpdate in case other scripts also use LateUpdate to change the transform
    private void LateUpdate()
    {
        if (transform.rotation != currentRotation)
        {
            targetRotation = transform.rotation;
        }

        if (transform.position != currentPosition)
        {
            targetPosition = transform.position;
        }

        if (lowPassFilter < 1)
        {
            if (applyOnRotation)
            {
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, lowPassFilter);
            }

            if (applyOnPosition)
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition, lowPassFilter);
            }
        }

        currentRotation = transform.rotation;
        currentPosition = transform.position;
    }
}
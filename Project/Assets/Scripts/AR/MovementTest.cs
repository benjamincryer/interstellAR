using UnityEngine;

public class MovementTest : MonoBehaviour
{
    private bool dirRight = true;
    public float speed = 2.0f;

    private void Update()
    {
        //Don't start moving until the object has been placed in correct position relative to camera
        if (GPS.latitude != 0 && GPS.longitude != 0)
        {
            if (dirRight)
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            else
                transform.Translate(-Vector2.right * speed * Time.deltaTime);

            if (transform.position.x >= 2.0f)
            {
                dirRight = false;
            }

            if (transform.position.x <= -2)
            {
                dirRight = true;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Android;

public class AppInit : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        //Request location permission
    #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    #endif
    }
}
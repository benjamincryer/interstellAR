using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPun
{
    [Header("Game Manager")]
    public Text mpstats;
    public GameObject camSpec;
    public GameObject camAR;

    public static List<Color> ColourList;

    [HideInInspector]
    //public ThirdPersonCharacter LocalPlayer;
    private bool gameStarted = false;

    private void Awake()
    {
        //Return to menu if we disconnect
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MainMenuAnimated");
            return;
        }
    }

    private void Start()
    {
        //AR mode on Android, Freecam mode on desktop
        camAR.SetActive(false);
        camSpec.SetActive(true);

#if UNITY_ANDROID
                camAR.SetActive(true);
                camSpec.SetActive(false);
#endif

        //manually setting this rn
        camAR.SetActive(false);
        camSpec.SetActive(true);

        //Set a bunch of distinct colors (8 available)
        ColourList = new List<Color>();
        ColourList.Add(new Color32(255, 25, 25, 112)); //Midnight Blue
        ColourList.Add(new Color32(255, 0, 100, 0)); //Dark Green
        ColourList.Add(new Color32(255, 255, 37, 0)); //Red
        ColourList.Add(new Color32(255, 255, 215, 0)); //Gold
        ColourList.Add(new Color32(255, 0, 255, 0)); //Lime Green
        ColourList.Add(new Color32(255, 0, 255, 255)); //Aqua
        ColourList.Add(new Color32(255, 255, 0, 255)); //Fuschia
        ColourList.Add(new Color32(255, 255, 182, 193)); //Light Pink
    }

    public void StartGame()
    {
        GameObject.Find("LobbyCanvas").SetActive(false);

        //if (Input.location.status == LocationServiceStatus.Running)
        //{
        //Instantiate player indicator object, shows location of other players
        PhotonNetwork.Instantiate("PlayerLocation", Vector3.zero, Quaternion.identity);

            //If we are the Master client and the GPS system is currently running, then generate Galaxy
            if (PhotonNetwork.IsMasterClient)
            {
                //Start Room timer
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("startTime", PhotonNetwork.Time);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                GetComponent<GalaxyGenerator>().enabled = true;
                gameStarted = true;
            }
        //}

        List<int> ColoursUsed = new List<int>();
        int players = PhotonNetwork.PlayerList.Length;

        //No more than 8 players in a game
        if (players > 8) PhotonNetwork.Disconnect();

        //Retrieve list of existing player colors
        for (int i = 0; i < players; i++)
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("colour"))
            {
                ColoursUsed.Add((int)PhotonNetwork.PlayerList[i].CustomProperties["colour"]);
            }
        }

        //Randomly generate unique Color from list
        int newColour;
        while (true)
        {
            newColour = Random.Range(0, ColourList.Count);
            if (!ColoursUsed.Contains(newColour)) break; //Allow colour if it doesn't exist
        }

        Debug.Log("COLOUR INDEX: " + newColour);

        //PLAYER PROPERTIES
        ExitGames.Client.Photon.Hashtable hashPlayer = new ExitGames.Client.Photon.Hashtable();

        //Set initial player Score and Colour
        hashPlayer.Add("score", 0);
        hashPlayer.Add("colour", newColour);

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashPlayer);



        //Generate space station at current player location
        var station = PhotonNetwork.Instantiate("Prefabs/SpaceObjects/SpaceStation", Camera.main.gameObject.transform.position,Quaternion.identity);

        station.GetComponent<Planet>().Metal.ResourceRate /= 2 ; //lower resource rate than planets

        //Set 100% capture points by default
        station.GetPhotonView().RPC("SetCapturePoints", RpcTarget.AllBuffered, 100f as object);
        station.GetPhotonView().RPC("InitAttributesShort", RpcTarget.AllBuffered, 1000f, "Space Station: " + PhotonNetwork.LocalPlayer.NickName, 10f, 10f);

    }

    void Update()
    {
        /*
        //Print player stats
        mpstats.text = "";
        //Loop through all players in the room
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            mpstats.text += "Player " + i + ": " + PhotonNetwork.PlayerList[i].NickName;
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName) { mpstats.text += "(YOU)"; } //highlight if player is you
            mpstats.text += "\n";
        }

        //Show/hide start button depending on host
        if (PhotonNetwork.IsMasterClient && gameStarted == false) //&& PhotonNetwork.CurrentRoom.PlayerCount > 1 (2 or more players)
        {
            GameObject canvas = GameObject.Find("LobbyCanvas");
            if(canvas != null)
            {
                canvas.SetActive(true);
            }
        }*/
    }

}

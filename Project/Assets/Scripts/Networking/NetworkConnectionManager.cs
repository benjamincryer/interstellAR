using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks
{
    public Button btnConnectMaster;
    public Button btnConnectRoom;

    public bool attemptConnectMaster;
    public bool attemptConnectRoom;

    public bool quickConnect = false;
    public string sceneLoad = "ARScene"; //default scene to load, ie. in actual game

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        attemptConnectMaster = false;
        attemptConnectRoom = false;

        //Start connecting to Master
        if (quickConnect) OnClickConnectMaster();
    }

    // Update is called once per frame
    void Update()
    {
        if (btnConnectMaster != null)
            btnConnectMaster.gameObject.SetActive(!PhotonNetwork.IsConnected && !attemptConnectMaster);

        if (btnConnectRoom != null)
            btnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !attemptConnectMaster && !attemptConnectRoom);

        //IF QUICK CONNECT ENABLED
        if (quickConnect)
        {
            //If Master finished connecting, then connect to Room
            if (!attemptConnectMaster) OnClickConnectRoom();
        }

    }

    public void OnClickConnectMaster()
    {
        //Run in Offline mode if no connection found
        if (Application.internetReachability == NetworkReachability.NotReachable || quickConnect)
        {
            PhotonNetwork.OfflineMode = true;
            Debug.Log("No internet connection");
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
        }

        //PhotonNetwork.OfflineMode = false;

        //Set name to random string
        string namestr = "";
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        for (int i = 0; i < 5; i++)
        {
            namestr += glyphs[Random.Range(0, glyphs.Length)];
        }

        PhotonNetwork.NickName = namestr;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "v1.0.0";

        attemptConnectMaster = true;
        
        if (PhotonNetwork.OfflineMode == false) PhotonNetwork.ConnectUsingSettings();
        else PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        attemptConnectMaster = false;
        attemptConnectRoom = false;
        Debug.Log(cause);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        attemptConnectMaster = false;
        Debug.Log("Connected to Master");
    }

    public void OnClickConnectRoom()
    {
        if (!PhotonNetwork.IsConnected) return;

        attemptConnectRoom = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        attemptConnectRoom = false;

        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " Players in Room " + PhotonNetwork.CurrentRoom.Name + " Room Name: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
        
        PhotonNetwork.LoadLevel(sceneLoad);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //Create room if none exists
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        attemptConnectRoom = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
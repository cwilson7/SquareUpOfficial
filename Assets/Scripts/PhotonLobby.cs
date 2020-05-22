using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject startButton, loadingTxt, chooseModePnl, startPnl, createRoomPnl, currentPnl;

    private void Awake()
    {
        lobby = this;
    }

    void Start()
    {
        currentPnl = startPnl;
        // Once we move to server implementation, this needs to be changed to "PhotonNetwork.ConnectToMaster(IP of server, port of server, our decided name of server);"
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        MultiplayerSettings.multiplayerSettings.InitializeCustomProperties();
        Loading(false);
    }

    

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room.");
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating Room.");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log("Created room with number: " + randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room. Attempting again...");
        CreateRoom();
    }

    public void StartRandomLobby()
    {
        Loading(true);
        PhotonNetwork.JoinRandomRoom(null, (byte)MultiplayerSettings.multiplayerSettings.maxPlayers);
    }

    private void Loading(bool isLoading)
    {
        startButton.SetActive(!isLoading);
        loadingTxt.SetActive(isLoading);
    }
    
    public void CaseSwitchPanels(int transition)
    {
        Debug.Log("button presesed");
        switch(transition)
        {
            case 0:
                SwitchPanels(chooseModePnl);
                break;
            case 1:
                SwitchPanels(createRoomPnl);
                break;
            case 2:
                SwitchPanels(startPnl);
                break;
        }
    } 

    private void SwitchPanels(GameObject pnlIn)
    {
        pnlIn.SetActive(true);
        currentPnl.SetActive(false);
        currentPnl = pnlIn;
    }
}

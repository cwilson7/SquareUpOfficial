using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Net;
using TMPro;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject startButton, loadingTxtPrefab, joinRndLobbyBtn, chooseModePnl, startPnl, createRoomPnl, currentPnl;
    [SerializeField] Canvas canvas;
    private List<GameObject> loadingPrefabs;

    private void Awake()
    {
        lobby = this;
    }

    void Start()
    {
        currentPnl = startPnl;
        foreach (TMP_Text text in canvas.GetComponentsInChildren<TMP_Text>())
        {
            text.font = MultiplayerSettings.multiplayerSettings.font;
        }
        loadingPrefabs = new List<GameObject>();
        GameObject startLoading = Instantiate(loadingTxtPrefab, startButton.transform.parent.transform);
        GameObject lobbyLoading = Instantiate(loadingTxtPrefab, joinRndLobbyBtn.transform.parent.transform);
        loadingPrefabs.Add(startLoading);
        loadingPrefabs.Add(lobbyLoading);
        startButton.SetActive(false);
        chooseModePnl.SetActive(false);
        createRoomPnl.SetActive(false);
        // Once we move to server implementation, this needs to be changed to "PhotonNetwork.ConnectToMaster(IP of server, port of server, our decided name of server);"
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        MultiplayerSettings.multiplayerSettings.InitializeCustomProperties();

        PhotonNetwork.JoinLobby(); 
        Loading(false, startButton);
    }

    public void JoinSelectedRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
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

    public void CreateCustomRoom()
    {
        if (MultiplayerSettings.multiplayerSettings.customRoomName == null)
        {
            Debug.Log("Name the room");
            return;
        }
        RoomOptions roomOps = new RoomOptions() { IsVisible = !MultiplayerSettings.multiplayerSettings.customRoomPrivate, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom(MultiplayerSettings.multiplayerSettings.customRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room. Attempting again...");
        CreateRoom();
    }

    public void StartRandomLobby()
    {
        Loading(true, joinRndLobbyBtn);
        PhotonNetwork.JoinRandomRoom(null, (byte)MultiplayerSettings.multiplayerSettings.maxPlayers);
    }

    private void Loading(bool isLoading, GameObject btnToCover)
    {
        btnToCover.SetActive(!isLoading);
        foreach(GameObject loading in loadingPrefabs)
        {
            loading.SetActive(isLoading);
        }
    }
    
    public void CaseSwitchPanels(int transition)
    {
        switch(transition)
        {
            case 0:
                SwitchPanels(chooseModePnl);
                Loading(false, joinRndLobbyBtn);
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

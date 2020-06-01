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

    public GameObject startButton, loadingTxtPrefab, roomNotFoundTxtPrefab, roomListContent, joinRndLobbyBtn, chooseModePnl, startPnl, createRoomPnl, currentPnl;
    [SerializeField] Canvas canvas;
    private Hashtable loadingObjects;

    private GameObject roomNotFoundTxt;
    private bool inPhotonLobby;
    
    private void Awake()
    {
        lobby = this;
    }
    #region Networking
    void Start()
    {
        InitializeLoadingUI();
        Loading(true, startButton);
        // Once we move to server implementation, this needs to be changed to "PhotonNetwork.ConnectToMaster(IP of server, port of server, our decided name of server);"
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        MultiplayerSettings.multiplayerSettings.InitializeCustomProperties();

        PhotonNetwork.JoinLobby(); 
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        CachedRoomList.cachedRoomList.rooms.Clear();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        inPhotonLobby = true;
        Loading(false, startButton);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        inPhotonLobby = false;
    }

    public void JoinSelectedRoom(RoomInfo info)
    {
        StartCoroutine(LeaveLobbyDelay());
        PhotonNetwork.JoinRoom(info.Name);
    }

    IEnumerator LeaveLobbyDelay()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            Loading(true, roomListContent);
            yield return new WaitForSeconds(1f);
            if (inPhotonLobby == true) StartCoroutine(LeaveLobbyDelay());
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room.");
        CreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Loading(false, roomListContent);
        PhotonNetwork.JoinLobby();
        roomNotFoundTxt = Instantiate(roomNotFoundTxtPrefab, currentPnl.transform);
        StartCoroutine(TextFade());
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

    public void SearchForRoom(string roomName)
    {
        StartCoroutine(LeaveLobbyDelay());
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion

    #region UI Elements
    IEnumerator TextFade()
    {
        if (roomNotFoundTxt != null)
        {
            TMP_Text currentTxt = roomNotFoundTxt.GetComponent<TMP_Text>();
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / 16;
                currentTxt.alpha = Mathf.Lerp(currentTxt.alpha, 0, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            Destroy(currentTxt);
        }
    }
    
    private void Loading(bool isLoading, GameObject item)
    {
        item.SetActive(!isLoading);
        GameObject loading = (GameObject)loadingObjects[item];
        loading.SetActive(isLoading);
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

    private void InitializeLoadingUI()
    {
        foreach (TMP_Text text in canvas.GetComponentsInChildren<TMP_Text>())
        {
            text.font = MultiplayerSettings.multiplayerSettings.font;
        }
        
        currentPnl = startPnl;
        chooseModePnl.SetActive(false);
        createRoomPnl.SetActive(false);

        loadingObjects = new Hashtable();
        PopulateLoadingHash();
    }

    private void SetLoadingHash(GameObject key)
    {
        GameObject loadingTxt = Instantiate(loadingTxtPrefab, key.transform.parent.transform);
        loadingObjects.Add(key, loadingTxt);
        loadingTxt.SetActive(false);
    }

    private void PopulateLoadingHash()
    {
        SetLoadingHash(startButton);
        SetLoadingHash(roomListContent);
        SetLoadingHash(joinRndLobbyBtn);
    }
    #endregion
}

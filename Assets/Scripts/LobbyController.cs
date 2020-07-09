using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public static LobbyController lc;

    public List<GameObject> charAvatars;
    public List<Material> availableMaterials;
    public List<int> selectedMaterialIDs;
    public List<int> IDsOfDisconnectedPlayers;
    public int currMasterID;

    // Start is called before the first frame update
    void Awake()
    {
        if (LobbyController.lc == null)
        {
            LobbyController.lc = this;
        }
        else
        {
            if (LobbyController.lc != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
        InitalizeInfoLists();
    }

    // Update is called once per frame
    void InitalizeInfoLists()
    {
        charAvatars = new List<GameObject>();
        availableMaterials = new List<Material>();
        IDsOfDisconnectedPlayers = new List<int>();
        currMasterID = PhotonNetwork.MasterClient.ActorNumber;

        Object[] avatarPrefabs = Resources.LoadAll("PhotonPrefabs/CharacterAvatars");
        foreach (Object prefab in avatarPrefabs)
        {
            GameObject prefabGO = (GameObject)prefab;
            charAvatars.Add(prefabGO);
        }

        Object[] mats = Resources.LoadAll("PhotonPrefabs/Materials");
        foreach (Object mat in mats)
        {
            Material prefabGO = (Material)mat;
            availableMaterials.Add(prefabGO);
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }

    public void ReturnToMenu()
    {
        Destroy(PhotonRoom.room.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        Destroy(this.gameObject);
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.mainMenuScene);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(MultiplayerSettings.multiplayerSettings.intermediateScene))
        {
            //nothing?
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(MultiplayerSettings.multiplayerSettings.multiplayerScene))
        {
            if (!IDsOfDisconnectedPlayers.Contains(otherPlayer.ActorNumber)) IDsOfDisconnectedPlayers.Add(otherPlayer.ActorNumber);
        }
        int myColorID = (int)otherPlayer.CustomProperties["AssignedColor"];
        if (LobbyController.lc.selectedMaterialIDs.Contains(myColorID))
        {
            LobbyController.lc.selectedMaterialIDs.Remove(myColorID);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(MultiplayerSettings.multiplayerSettings.intermediateScene))
        {
            //nothing?
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(MultiplayerSettings.multiplayerSettings.multiplayerScene))
        {
            IDsOfDisconnectedPlayers.Add(currMasterID);
            currMasterID = newMasterClient.ActorNumber;
            if (GameInfo.GI != null)
            {
                if (GameInfo.GI.TimeStopped == false) GameInfo.GI.StopTime();
                Score playerInfo = (Score)GameInfo.GI.scoreTable[newMasterClient.ActorNumber];
                PhotonPlayer newHost = playerInfo.photonPlayer.GetComponent<PhotonPlayer>();
                newHost.makingCubeClone = true;
                newHost.SetUpCube();
            }
        }
        //display migrating host text
    }

    [PunRPC]
    public void UpdateAllCharacters_RPC()
    {
        StartCoroutine(GameObject.Find("PlayerList").GetComponent<PlayerListController>().InformationDelay(true));//UpdatePlayerListingsAndUsedColorList(PhotonNetwork.LocalPlayer, true);
        GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>().UpdateCurrentDisplayedCharacter();
    }

    [PunRPC]
    public void ResetColorInfo_RPC(int actorNr, int colorID)
    {
        //StartCoroutine(GameObject.Find("PlayerList").GetComponent<PlayerListController>().InformationDelay(true));
        if (PhotonNetwork.LocalPlayer.ActorNumber != actorNr) return;
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("AssignedColor", colorID);
        //MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"] = colorID;
    }

    [PunRPC]
    public void SetPlayerValuesTest_RPC()
    {
        PlayerListController plc = GameObject.Find("PlayerList").GetComponent<PlayerListController>();
        CharSelectPanelController cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();

        cspc.SendToPlayerList();
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("PlayerReady", true);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", 5);
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["AssignedColor"] == -1) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("AssignedColor", 0);
        //cspc.UpdateCurrentDisplayedCharacter();
        //lbc.waitingTxt.enabled = (bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"];
        StartCoroutine(plc.InformationDelay(false));
    }
}
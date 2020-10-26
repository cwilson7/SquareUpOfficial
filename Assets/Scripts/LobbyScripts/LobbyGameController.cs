using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyGameController : MonoBehaviour
{
    public TMP_Text waitingTxt, playerCounterTxt, voteCounterTxt, votedTxt;
    public GameObject forceStartBtn;
    bool forceStart = false, starting = false, allReady = false;
    int forceStartVotes = 0;

    // Start is called before the first frame update
    void Start()
    {
        waitingTxt.enabled = false;
    }

    private void FixedUpdate()
    {
        if (starting) return;
        if (PhotonNetwork.IsConnected && (bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] && !allReady) CheckIfAllReady();
        if (PhotonNetwork.IsConnected) CheckForVote();
        if (!starting && allReady && (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers || forceStart)) StartCoroutine(StartingGame()); 
        if (PhotonNetwork.IsConnected) playerCounterTxt.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + " players";
        voteCounterTxt.text = forceStartVotes + " votes to start";
    }

    void CheckForVote()
    {
        if (PhotonNetwork.IsMasterClient) forceStart = true;
        forceStartVotes = PhotonNetwork.CurrentRoom.PlayerCount;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["VoteForceStart"])
            {
                if (PhotonNetwork.IsMasterClient) forceStart = false;
                forceStartVotes--;
            }
        }
    }

    private void CheckIfAllReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["PlayerReady"] || (int)player.CustomProperties["AssignedColor"] == -1 || (int)player.CustomProperties["SelectedCharacter"] == -1)
            {
                allReady = false;
            }
        }
    }

    public void VoteStart()
    {
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("VoteForceStart", true);
        forceStartBtn.SetActive(false);
        votedTxt.gameObject.SetActive(true);
    }

    IEnumerator StartingGame()
    {
        //Aesthetic changes
        starting = true;
        yield return new WaitForSeconds(1f);
        if (GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>().CheckForDuplicateMaterials())
        {
            StartCoroutine(StartingGame());
        }
        else
        {
            LobbyController.lc.gameObject.GetComponent<PhotonView>().RPC("UpdateAllCharacters_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            LoadingPanel.loadingPanel.DisplayLoadingPanel(true);
            yield return new WaitForSeconds(5f);
            LobbyController.lc.StartGame();
        }
    }
}

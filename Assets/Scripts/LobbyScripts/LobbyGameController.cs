using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyGameController : MonoBehaviour
{
    public TMP_Text waitingTxt;
    private bool allReady;
    
    // Start is called before the first frame update
    void Start()
    {
        waitingTxt.enabled = false;
        allReady = false;
    }

    private void FixedUpdate()
    {
        if(PhotonNetwork.IsConnected && (bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"] && !allReady) CheckIfAllReady();
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
        if (allReady) StartCoroutine(StartingGame());
    }

    IEnumerator StartingGame()
    {
        Debug.Log("Starting game");
        //Aesthetic changes
        PhotonNetwork.CurrentRoom.IsOpen = false;
        yield return new WaitForSeconds(5f);
        LobbyController.lc.StartGame();
    }
}

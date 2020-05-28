using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyGameController : MonoBehaviour
{
    public TMP_Text waitingTxt;
    
    // Start is called before the first frame update
    void Start()
    {
        waitingTxt.enabled = false;
    }

    private void FixedUpdate()
    {
        if((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"]) CheckIfAllReady();
    }

    private void CheckIfAllReady()
    {
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["PlayerReady"])
            {
                allReady = false;
            }
        }
        if (allReady) StartCoroutine(StartingGame());
    }

    IEnumerator StartingGame()
    {
        yield return new WaitForSeconds(5f);
        LobbyController.lc.StartGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MiniMapPlayer : MonoBehaviour
{
    Controller ParentController;
    private PhotonView PV;
    public bool miniMapSet = false, stopUpdateCalls = false;

    // Start is called before the first frame update

    public void InitializeMiniMapIndicator()
    {
        ParentController = GetComponentInParent<Controller>();
        PV = GetComponentInParent<PhotonView>();
        PV.RPC("SetUpMiniMap_RPC", RpcTarget.AllBuffered, ParentController.actorNr);
        Debug.Log("i am setting loadded in to true biottttttch");
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("LoadedIn", true);
    }

    private void FixedUpdate()
    {
        if (!miniMapSet && !stopUpdateCalls) CheckIfAllInGameInfo();
    }

    private void CheckIfAllInGameInfo()
    {
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["GameInfoSet"])
            {
                allReady = false;
            }
        }
        if (allReady)
        {
            InitializeMiniMapIndicator();
            stopUpdateCalls = true;
        }
    }

}

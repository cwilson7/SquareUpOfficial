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

    public void OnCubeStateChangeMap(bool startingRotation, float newZ)
    {
        if (startingRotation)
        {
            transform.localScale *= 2;
            transform.localPosition = Vector3.zero;
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            gameObject.layer = LayerMask.NameToLayer("Player");
            MiniMapCamera.mmCamera.projectedBlank.enabled = false;
        }
        else
        {
            transform.localScale /= 2;
            transform.localPosition = Vector3.zero;
            gameObject.layer = LayerMask.NameToLayer("MiniMap");
            MiniMapCamera.mmCamera.projectedBlank.enabled = true;
        }
    }

}

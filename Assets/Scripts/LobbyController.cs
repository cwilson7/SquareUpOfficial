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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
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
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.mainMenuScene);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        int myColorID = (int)otherPlayer.CustomProperties["AssignedColor"];
        if(LobbyController.lc.selectedMaterialIDs.Contains(myColorID))
        {
            LobbyController.lc.selectedMaterialIDs.Remove(myColorID);
        }
    }
}

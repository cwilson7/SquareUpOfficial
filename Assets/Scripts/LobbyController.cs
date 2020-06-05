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
        InitalizeInfoLists();
    }

    // Update is called once per frame
    void InitalizeInfoLists()
    {
        charAvatars = new List<GameObject>();
        availableMaterials = new List<Material>();
        
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
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.mainMenuScene);
        Destroy(this);
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

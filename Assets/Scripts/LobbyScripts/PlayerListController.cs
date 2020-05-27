using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using System.Linq;

public class PlayerListController : MonoBehaviourPunCallbacks
{
    private List<PlayerListing> playerListings;
    [SerializeField] private PlayerListing playerListingPrefab;
    private CharSelectPanelController cspc;

    [SerializeField] private Button resetCharBtn;

    private void Start()
    {
        InitializePlayerListings();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        PlayerListing listing = Instantiate(playerListingPrefab, transform);
        if(listing != null)
        {
            listing.SetPlayerListing(player);
            playerListings.Add(listing);
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        int index = playerListings.FindIndex(x => x.Player == player);
        if(index  != -1)
        {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);        
        }
    }

    private void InitializePlayerListings()
    {
        cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();
        resetCharBtn.onClick.AddListener(ResetPlayerInfo);
        playerListings = new List<PlayerListing>();

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            PlayerListing listing = Instantiate(playerListingPrefab, transform);
            listing.SetPlayerListing(p);
            playerListings.Add(listing);
        }
    }

    public void UpdatePlayerListingsAndUsedColorList(Player p)
    {
        GetComponent<PhotonView>().RPC("SetPlayerInfo_RPC", RpcTarget.AllBuffered, p.ActorNumber);
    }

    private void ResetPlayerInfo()
    {
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("PlayerReady", false);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", -1);
        cspc.UpdateCurrentDisplayedCharacter();
        cspc.SendToFirstCharacterPanel();
        StartCoroutine(InformationDelay());
    }

    IEnumerator InformationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        UpdatePlayerListingsAndUsedColorList(PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void SetPlayerInfo_RPC(int actorNumber)
    {
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if (!LobbyController.lc.selectedMaterialIDs.Contains((int)p.CustomProperties["AssignedColor"])) {
            LobbyController.lc.selectedMaterialIDs.Add((int)p.CustomProperties["AssignedColor"]);
        }

        int index = playerListings.FindIndex(x => x.Player == p);
        if (index != -1)
        {
            playerListings[index].SetPlayerListing(p);
        }
    }
}

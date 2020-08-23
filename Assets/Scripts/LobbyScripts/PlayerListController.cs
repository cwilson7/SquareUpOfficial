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
        if (listing != null)
        {
            listing.SetPlayerListing(player, false);
            playerListings.Add(listing);
        }
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        int index = playerListings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }

    private void InitializePlayerListings()
    {
        cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();
        //resetCharBtn.onClick.AddListener(ResetPlayerInfo);
        playerListings = new List<PlayerListing>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            PlayerListing listing = Instantiate(playerListingPrefab, transform);
            listing.SetPlayerListing(p, false);
            playerListings.Add(listing);
        }
    }

    public void UpdatePlayerListingsAndUsedColorList(Player p, bool showColor)
    {
        GetComponent<PhotonView>().RPC("SetPlayerInfo_RPC", RpcTarget.AllBuffered, p.ActorNumber, showColor);
    }

    private void ResetPlayerInfo()
    {
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("PlayerReady", false);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", -1);
        cspc.SendToFirstCharacterPanel();
        StartCoroutine(InformationDelay(false));
    }

    public IEnumerator InformationDelay(bool showColor)
    {
        yield return new WaitForSeconds(0.5f);
        Player local = PhotonNetwork.LocalPlayer;
        if ((int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"] != (int)local.CustomProperties["AssignedColor"] || (int)MultiplayerSettings.multiplayerSettings.localPlayerValues["SelectedCharacter"] != (int)local.CustomProperties["SelectedCharacter"]) StartCoroutine(InformationDelay(showColor));
        else UpdatePlayerListingsAndUsedColorList(local, showColor);
    }

    [PunRPC]
    private void SetPlayerInfo_RPC(int actorNumber, bool showColor)
    {
        
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if (!LobbyController.lc.selectedMaterialIDs.Contains((int)p.CustomProperties["AssignedColor"]))
        {
            LobbyController.lc.selectedMaterialIDs.Add((int)p.CustomProperties["AssignedColor"]);
        }

        int index = playerListings.FindIndex(x => x.Player == p);
        if (index != -1)
        {
            playerListings[index].SetPlayerListing(p, showColor);
        }
    }
}

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
        playerListings = new List<PlayerListing>();

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            PlayerListing listing = Instantiate(playerListingPrefab, transform);
            listing.SetPlayerListing(p);
            playerListings.Add(listing);
        }
    }

    public void UpdatePlayerListings(Player p)
    {
        GetComponent<PhotonView>().RPC("SetPlayerInfo_RPC", RpcTarget.AllBuffered, p.ActorNumber);
    }

    [PunRPC]
    private void SetPlayerInfo_RPC(int actorNumber)
    {
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        int index = playerListings.FindIndex(x => x.Player == p);
        if (index != -1)
        {
            Debug.Log("Ready: " + (bool)p.CustomProperties["PlayerReady"]);
            playerListings[index].SetPlayerListing(p);
        }
    }
}

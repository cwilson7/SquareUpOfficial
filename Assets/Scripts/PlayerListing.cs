using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] TMP_Text playerLabel;

    public Player Player { get; private set; }

    public void SetPlayerListing(Player p)
    {
        Player = p;
        playerLabel.text = p.NickName;
        if ((bool)p.CustomProperties["PlayerReady"])
        {
            playerLabel.color = LobbyController.lc.availableMaterials[(int)p.CustomProperties["AssignedColor"]].color;
            playerLabel.text += " - " + LobbyController.lc.charAvatars[(int)p.CustomProperties["SelectedCharacter"]].name;
        }
    }

}

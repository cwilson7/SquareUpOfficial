using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] TMP_Text playerLabel;

    public Player Player { get; private set; }

    public void SetPlayerListing(Player p, bool showColor)
    {
        Player = p;
        playerLabel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.parent.GetComponent<RectTransform>().rect.width);
        playerLabel.text = p.NickName;
        playerLabel.font = MultiplayerSettings.multiplayerSettings.font;
        if ((bool)p.CustomProperties["PlayerReady"])
        {
            if (showColor) playerLabel.color = LobbyController.lc.availableMaterials[(int)p.CustomProperties["AssignedColor"]].color;
            Debug.Log("character index selected: " + (int)p.CustomProperties["SelectedCharacter"]);
            Debug.Log("length of charAvatars: " + LobbyController.lc.charAvatars.Count);
            playerLabel.text += " - " + LobbyController.lc.charAvatars[(int)p.CustomProperties["SelectedCharacter"]].GetComponent<AvatarCharacteristics>().info.characterName;
        }
        else
        {
            //MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", -1);
            Debug.Log("setting shit to -1");
        }
    }

}

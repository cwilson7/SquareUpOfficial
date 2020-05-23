using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] private TMP_Text roomLabel, numInLobbyLabel;
    [SerializeField] private Button roomBtn;
    [SerializeField] private int fontSizeBig, fontSizeSmall;

    public RoomInfo RoomInfo { get; private set; }

    private void Awake()
    {
        roomBtn.onClick.AddListener(JoinRoom);
    }

    public void SetRoomListing(RoomInfo r)
    {
        RoomInfo = r;

        roomBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.parent.GetComponent<RectTransform>().rect.width);

        roomLabel.fontSize = fontSizeBig;
        numInLobbyLabel.fontSize = fontSizeSmall;
        roomLabel.font = MultiplayerSettings.multiplayerSettings.font;
        numInLobbyLabel.font = MultiplayerSettings.multiplayerSettings.font;

        roomLabel.text = r.Name;
        numInLobbyLabel.text = r.PlayerCount + "/" + r.MaxPlayers;
    }

    private void JoinRoom()
    {
        PhotonLobby.lobby.JoinSelectedRoom(RoomInfo);
    }
}

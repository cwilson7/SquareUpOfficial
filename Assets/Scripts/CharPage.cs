using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CharPage : MonoBehaviour
{
    [SerializeField] private TMP_Text charName;
    [SerializeField] private Button charSelectBtn;

    private int myCharID;
    private PlayerListController plc;
    private CharSelectPanelController cspc;

    private void Start()
    {
        charSelectBtn.onClick.AddListener(SetPlayerInfo);
        plc = GameObject.Find("PlayerList").GetComponent<PlayerListController>();
        cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();
    }

    public void ShowDetails(int charID)
    {
        myCharID = charID;
        
        charName.font = MultiplayerSettings.multiplayerSettings.font;
        charSelectBtn.GetComponentInChildren<TMP_Text>().font = MultiplayerSettings.multiplayerSettings.font;

        charName.text = LobbyController.lc.charAvatars[charID].name;
        charSelectBtn.GetComponentInChildren<TMP_Text>().text = "Select " + LobbyController.lc.charAvatars[charID].name;
    }

    private void SetPlayerInfo()
    {
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("PlayerReady", true);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", myCharID);
        if((int)PhotonNetwork.LocalPlayer.CustomProperties["AssignedColor"] == -1) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("AssignedColor", GenerateRandomColorID());

        cspc.SendToPlayerList();
        StartCoroutine(InformationDelay());
    }

    private IEnumerator InformationDelay()
    {
        //Takes a little for hashtable to change key: "PlayerReady" to value true
        yield return new WaitForSeconds(0.5f);
        plc.UpdatePlayerListings(PhotonNetwork.LocalPlayer);
    }

    private int GenerateRandomColorID()
    {
        int maxColors = Mathf.Min(MultiplayerSettings.multiplayerSettings.maxPlayers, LobbyController.lc.availableMaterials.Count);
        int color = Random.Range(0, maxColors);
        return color;
    }
}

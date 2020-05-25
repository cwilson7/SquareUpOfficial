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
    [SerializeField] private float distanceFromCamera;

    private int myCharID;
    private PlayerListController plc;
    private CharSelectPanelController cspc;
    private Camera characterCamera;

    private void Awake()
    {
        charSelectBtn.onClick.AddListener(SetPlayerInfo);
        plc = GameObject.Find("PlayerList").GetComponent<PlayerListController>();
        cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();
        characterCamera = cspc.charDisplayCamera;
    }

    public void ShowDetails(int charID)
    {
        myCharID = charID;
        
        charName.font = MultiplayerSettings.multiplayerSettings.font;
        charSelectBtn.GetComponentInChildren<TMP_Text>().font = MultiplayerSettings.multiplayerSettings.font;

        charName.text = LobbyController.lc.charAvatars[charID].name;
        charSelectBtn.GetComponentInChildren<TMP_Text>().text = "Select " + LobbyController.lc.charAvatars[charID].name;

        Transform characterDisplayPos = CarouselController.cc.carousel.GetComponent<CarouselBehaviour>().playerDisplayLocations[charID];  
        GameObject character = Instantiate(LobbyController.lc.charAvatars[charID], characterDisplayPos);
        character.layer = 9;
        cspc.displayedCharacters.Add(charID, character);
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
        cspc.UpdateCurrentDisplayedCharacter();
        plc.UpdatePlayerListingsAndUsedColorList(PhotonNetwork.LocalPlayer);
    }

    private int GenerateRandomColorID()
    {
        int maxColors = Mathf.Min(MultiplayerSettings.multiplayerSettings.maxPlayers, LobbyController.lc.availableMaterials.Count);
        int color = Random.Range(0, maxColors);
        if(LobbyController.lc.selectedMaterialIDs.Contains(color))
        {
            return GenerateRandomColorID();
        }
        else return color;
    }
}

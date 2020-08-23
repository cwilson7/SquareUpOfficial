using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CharPage : MonoBehaviour
{
    [SerializeField] private TMP_Text charName, lockedText;
    [SerializeField] private Button charSelectBtn;
    [SerializeField] private float distanceFromCamera;
    [SerializeField] public GameObject myPanel;
    [SerializeField] private List<GameObject> panelElements;

    private int myCharID;
    private PlayerListController plc;
    private CharSelectPanelController cspc;
    private LobbyGameController lbc;
    private Camera characterCamera;

    private void Awake()
    {
        charSelectBtn.onClick.AddListener(SetPlayerInfo);
        plc = GameObject.Find("PlayerList").GetComponent<PlayerListController>();
        cspc = GameObject.Find("CharSelectPanelContainer").GetComponent<CharSelectPanelController>();
        lbc = GameObject.Find("GameController").GetComponent<LobbyGameController>();
        foreach (Transform UIElement in myPanel.GetComponentsInChildren<Transform>())
        {
            panelElements.Add(UIElement.gameObject);
        }
        characterCamera = cspc.charDisplayCamera;
    }

    public void ShowDetails(int charID)
    {
        myCharID = charID;
        
        charName.font = MultiplayerSettings.multiplayerSettings.font;
        charSelectBtn.GetComponentInChildren<TMP_Text>().font = MultiplayerSettings.multiplayerSettings.font;

        charName.text = LobbyController.lc.charAvatars[charID].GetComponent<AvatarCharacteristics>().info.characterName;// name;
        charSelectBtn.GetComponentInChildren<TMP_Text>().text = "Select";

        Transform characterDisplayPos = CarouselController.cc.carousel.GetComponent<CarouselBehaviour>().playerDisplayLocations[charID];  
        GameObject character = Instantiate(LobbyController.lc.charAvatars[charID], characterDisplayPos);
        foreach(Transform trans in character.GetComponentsInChildren<Transform>())
        {
            trans.gameObject.layer = 9;
        }
        cspc.displayedCharacters.Add(charID, character);

        CharacterInfo info = (CharacterInfo)ProgressionSystem.Instance.Characters[LobbyController.lc.charAvatars[charID].GetComponent<AvatarCharacteristics>().info.characterName];
        character.GetComponent<AvatarCharacteristics>().DisplayCosmetics();
        if (info.status == Status.Locked) SetToLockedPanel();
    }

    public void SetToLockedPanel()
    {
        panelElements.Add(lockedText.gameObject);
        panelElements.Remove(GetComponentInChildren<Button>().gameObject);
        lockedText.gameObject.SetActive(true);
        GetComponentInChildren<Button>().gameObject.SetActive(false);
    }

    public void SetPanelActive(bool isActive)
    {
        foreach(GameObject UIElement in panelElements)
        {
            if(UIElement != gameObject) UIElement.SetActive(isActive);
        }
    }

    public void TestButton()
    {
        LobbyController.lc.photonView.RPC("SetPlayerValuesTest_RPC", RpcTarget.All);
    }

    private void SetPlayerInfo()
    {
        cspc.SendToPlayerList();
        cspc.voteStartBtn.SetActive(true);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("PlayerReady", true);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("SelectedCharacter", myCharID);
        if((int)PhotonNetwork.LocalPlayer.CustomProperties["AssignedColor"] == -1) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("AssignedColor", GenerateRandomColorID());        
        lbc.waitingTxt.enabled = (bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"];
        StartCoroutine(plc.InformationDelay(false));
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

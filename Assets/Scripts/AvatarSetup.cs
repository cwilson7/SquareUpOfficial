using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    private GameObject avatarSkin;
    Controller controller;

    // Start is called before the first frame update
    public void Start()
    {
        InitializePlayerAvatar();
    }
    public void InitializePlayerAvatar()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine) PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void InitializeCharacter_RPC(int actorNumber)
    {        
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (LobbyController.lc.charAvatars.Count > (int)p.CustomProperties["SelectedCharacter"] || LobbyController.lc.availableMaterials.Count > (int)p.CustomProperties["AssignedColor"])
        {
            GameObject mySelectedCharacter = LobbyController.lc.charAvatars[(int)p.CustomProperties["SelectedCharacter"]];
            Material myAssignedColor = LobbyController.lc.availableMaterials[(int)p.CustomProperties["AssignedColor"]];

            avatarSkin = Instantiate(mySelectedCharacter, transform);
            avatarSkin.GetComponent<AvatarCharacteristics>().SetMaterial(myAssignedColor);

            AddPlayerController(avatarSkin);
            MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("CharacterSpawned", true);
        }
        else StartCoroutine(InformationDelay());
    }

    private void AddPlayerController(GameObject avatarGO)
    {
        Controller charControl = avatarGO.GetComponent<Controller>();
        controller = (Controller)gameObject.AddComponent(charControl.GetType());
        Destroy(charControl);
        controller.InitializePlayerController();
    }

    IEnumerator InformationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered);
    }
}

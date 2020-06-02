using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    private GameObject avatarSkin;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        AddPlayerController();
        if(PV.IsMine) PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void InitializeCharacter_RPC(int actorNumber)
    {
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (p == null) Debug.Log("player: " + actorNumber + " doesn't exist!");
        GameObject mySelectedCharacter = LobbyController.lc.charAvatars[(int)p.CustomProperties["SelectedCharacter"]];
        Material myAssignedColor = LobbyController.lc.availableMaterials[(int)p.CustomProperties["AssignedColor"]];

        avatarSkin = Instantiate(mySelectedCharacter, transform);
        avatarSkin.GetComponent<AvatarCharacteristics>().SetMaterial(myAssignedColor);
    }

    private void AddPlayerController()
    {
        switch((int)MultiplayerSettings.multiplayerSettings.localPlayerValues["SelectedCharacter"]+1)
        {
            case 1:
                gameObject.AddComponent<CharacterClass1>();
                break;
            case 2:
                gameObject.AddComponent<CharacterClass2>();
                break;
            case 3:
                gameObject.AddComponent<CharacterClass3>();
                break;
            case 4:
                gameObject.AddComponent<CharacterClass4>();
                break;
        }
    }
}

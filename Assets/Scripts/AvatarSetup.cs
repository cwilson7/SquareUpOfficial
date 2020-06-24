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
    [SerializeField] private float avatarOffset = 0.5f;

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
        int colorID = (int)p.CustomProperties["AssignedColor"];
        int charID = (int)p.CustomProperties["SelectedCharacter"];
        if (LobbyController.lc.charAvatars.Count >= charID && LobbyController.lc.availableMaterials.Count >= colorID && colorID >= 0 && charID >= 0)
        {
            GameObject mySelectedCharacter = LobbyController.lc.charAvatars[charID];
            Material myAssignedColor = LobbyController.lc.availableMaterials[colorID];

            avatarSkin = Instantiate(mySelectedCharacter, new Vector3(transform.position.x, transform.position.y - avatarOffset, transform.position.z), transform.rotation);
            avatarSkin.transform.SetParent(transform);
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
        PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }
}

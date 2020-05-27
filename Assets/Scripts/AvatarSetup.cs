using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine) PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void InitializeCharacter_RPC(int actorNumber)
    {
        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        GameObject mySelectedCharacter = LobbyController.lc.charAvatars[(int)p.CustomProperties["SelectedCharacter"]];
        Material myAssignedColor = LobbyController.lc.availableMaterials[(int)p.CustomProperties["AssignedColor"]];

        GameObject avatarSkin = Instantiate(mySelectedCharacter, transform, transform);
        avatarSkin.GetComponent<AvatarCharacteristics>().SetMaterial(myAssignedColor);
    }
}

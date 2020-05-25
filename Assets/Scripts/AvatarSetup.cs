using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    
    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void InitializeCharacter_RPC()
    {
        GameObject mySelectedCharacter = LobbyController.lc.charAvatars[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedCharacter"]];
        Material myAssignedColor = LobbyController.lc.availableMaterials[(int)PhotonNetwork.LocalPlayer.CustomProperties["AssignedColor"]];

        GameObject avatarSkin = Instantiate(mySelectedCharacter, transform, transform);
        avatarSkin.GetComponent<MeshRenderer>().sharedMaterial = myAssignedColor;
    }
}

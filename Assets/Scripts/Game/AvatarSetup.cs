﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;
using System.Linq;

public class AvatarSetup : MonoBehaviour
{
    public PhotonView PV;
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
        //avatar on file that matches my selected character id
        //grab the info i have saved for that character
        if (PV.IsMine) 
        {
            CharacterInfo myCharInfo = ProgressionSystem.CharacterData(LobbyController.lc.charAvatars[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedCharacter"]].GetComponent<AvatarCharacteristics>().info);
            PV.RPC("InitializeCharacter_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, myCharInfo.currentSet.NamesOfCosmetics().ToArray(), ProgressionSystem.playerData.myCrownName);
        }
    }

    [PunRPC]
    private void InitializeCharacter_RPC(int actorNumber, string[] cosmeticNames, string crownName)
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
            AvatarCharacteristics AC = avatarSkin.GetComponent<AvatarCharacteristics>();
            AC.SetMaterial(myAssignedColor);
            
            if (PV != null && PV.IsMine)// || PV.OwnerActorNr == actorNumber) //if this is setting up my character on my game
            {
                AC.info = ProgressionSystem.CharacterData(AC.info); //set my character data to my player data
                AC.DisplayAllCosmetics(); //display my cosmetics on my character
            }
            else AC.NetworkDisplayCosmetics(cosmeticNames.ToList()); //if this is setting up someone elses character on my game
            
            GameObject crown = AC.EquipCrown((GameObject)Resources.Load(ProgressionSystem.playerData.crownPath + crownName));

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
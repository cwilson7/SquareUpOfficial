using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSettings;
    private ExitGames.Client.Photon.Hashtable customProperties;
    public Hashtable localPlayerValues;

    public bool delayStart, customRoomPrivate;
    public int maxPlayers, multiplayerScene, intermediateScene, mainMenuScene;
    public string customRoomName;

    public TMP_FontAsset font; 

    private void Awake()
    {
        if(MultiplayerSettings.multiplayerSettings == null)
        {
            MultiplayerSettings.multiplayerSettings = this;
        }
        else
        {
            if(MultiplayerSettings.multiplayerSettings != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetMaxPlayers(string value)
    {
        int intVal = Int32.Parse(value);
        maxPlayers = intVal;
    }

    public void SetCustomRoomName(string value)
    {
        customRoomName = value;
    }

    public void SetRoomPrivate(bool isPrivate)
    {
        customRoomPrivate = isPrivate;
    }

    public void SetNickName(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Nickname set to: " + PhotonNetwork.NickName);
    }

    public void InitializeCustomProperties()
    {
        customProperties = new ExitGames.Client.Photon.Hashtable();
        localPlayerValues = new Hashtable();
        SetCustomPlayerProperties("PlayerReady", false);
        SetCustomPlayerProperties("SelectedCharacter", -1);
        SetCustomPlayerProperties("AssignedColor", -1);
    }
    
    public void SetCustomPlayerProperties(string key, object value)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key)) customProperties.Add(key, value);
        else customProperties[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

        if (localPlayerValues.ContainsKey(key)) localPlayerValues[key] = value;
        else localPlayerValues.Add(key, value);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSettings;
    private ExitGames.Client.Photon.Hashtable customProperties;

    public bool delayStart;
    public int maxPlayers, menuScene, multiplayerScene;

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

    public void SetMaxPlayers(int value)
    {
        Debug.Log("SetMaxPlayers called.");
        maxPlayers = value;
    }

    public void SetNickName(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        Debug.Log("Nickname set to: " + PhotonNetwork.NickName);
    }

    public void InitializeCustomProperties()
    {
        customProperties = new ExitGames.Client.Photon.Hashtable();
        SetPlayerReady(false);
        SetSelectedCharacter(-1);
        SetAssignedColor(-1);
    }
    
    public void SetPlayerReady(bool isReady)
    {
        customProperties["PlayerReady"] = isReady;
        PhotonNetwork.LocalPlayer.CustomProperties = customProperties;
    }

    public void SetSelectedCharacter(int charID)
    {
        customProperties["SelectedCharacter"] = charID;
        PhotonNetwork.LocalPlayer.CustomProperties = customProperties;
    }

    public void SetAssignedColor(int colorID)
    {
        customProperties["AssignedColor"] = colorID;
        PhotonNetwork.LocalPlayer.CustomProperties = customProperties;
    }
}

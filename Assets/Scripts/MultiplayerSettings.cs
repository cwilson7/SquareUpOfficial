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
        SetCustomPlayerProperties("PlayerReady", false);
        SetCustomPlayerProperties("SelectedCharacter", -1);
        SetCustomPlayerProperties("AssignedColor", -1);
    }
    
    public void SetCustomPlayerProperties(string key, object value)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key)) customProperties.Add(key, value);
        else customProperties[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

}

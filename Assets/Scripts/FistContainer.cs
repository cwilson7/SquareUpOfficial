using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistContainer : MonoBehaviour
{
    public void PurgeList()
    {
        
        List<int> playersLeft = LobbyController.lc.IDsOfDisconnectedPlayers;
        Fist[] fists = GetComponentsInChildren<Fist>();
        for (int i = 0; i < fists.Length; i++)
        {
            if (playersLeft.Contains(fists[i].owner))
            {
                fists[i].gameObject.SetActive(false);
            }
        }
        
    }
}

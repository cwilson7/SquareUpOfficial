using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int actorNumber;
    public Dictionary<Stat, int> playerStatistics;
    public GameObject photonPlayer, playerAvatar;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerStatistics = new Dictionary<Stat, int>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            playerStatistics.Add(stat, 0);
        }
    }

    public void AddToStat(Stat stat)
    {
        playerStatistics[stat] += 1;
    }
}

public enum Stat {
    kills, 
    deaths, 
    powerUpsCollected, 
    punchesThrown, 
    punchesLanded, 
    bulletsFired, 
    bulletsLanded, 
    falls
}

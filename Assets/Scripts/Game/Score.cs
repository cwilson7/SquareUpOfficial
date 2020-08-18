using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int actorNumber, kills, deaths, powerUpsCollected, punchesThrown, punchesLanded, bulletsFired, bulletsLanded, falls;

    public GameObject photonPlayer, playerAvatar;
    
    // Start is called before the first frame update
    void Awake()
    {
        kills = 0;
        deaths = 0;
        powerUpsCollected = 0;
        punchesThrown = 0;
        punchesLanded = 0;
        bulletsFired = 0;
        bulletsLanded = 0;
        falls = 0;
    }

    public void AddToStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.kills:
                kills += 1;
                break;
            case Stat.deaths:
                deaths += 1;
                break;
            case Stat.powerUpsCollected:
                powerUpsCollected += 1;
                break;
            case Stat.punchesThrown:
                punchesThrown += 1;
                break;
            case Stat.punchesLanded:
                punchesLanded += 1;
                break;
            case Stat.bulletsFired:
                bulletsFired += 1;
                break;
            case Stat.bulletsLanded:
                bulletsLanded += 1;
                break;
            case Stat.falls:
                falls += 1;
                break;
        }
    }

    public double Accuracy()
    {
        return (BulletAccuracy() + PunchAccuracy()) / 2;
    }

    public double BulletAccuracy()
    {
        return bulletsLanded / bulletsFired;
    }

    public double PunchAccuracy()
    {
        return punchesLanded / punchesThrown;
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

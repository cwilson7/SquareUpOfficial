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

    public void AddToStat(string stat)
    {
        switch (stat)
        {
            case "kills":
                kills += 1;
                break;
            case "deaths":
                deaths += 1;
                break;
            case "powerUpsCollected":
                powerUpsCollected += 1;
                break;
            case "punchesThrown":
                punchesThrown += 1;
                break;
            case "punchesLanded":
                punchesLanded += 1;
                break;
            case "bulletsFired":
                bulletsFired += 1;
                break;
            case "bulletsLanded":
                bulletsLanded += 1;
                break;
            case "falls":
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

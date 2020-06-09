using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform[] spawnPoints, weaponSpawnPoints, otherPowerUpSpawnPoints;
    public Transform face;
    public int num;

    public Transform[] ReturnArrayFromID(int id)
    {
        switch(id)
        {
            case 1:
                return weaponSpawnPoints;
            case 2:
                return otherPowerUpSpawnPoints;
        }
        return null;
    }
}

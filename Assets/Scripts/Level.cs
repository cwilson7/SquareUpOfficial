using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform[] spawnPoints, powerUpSpawnPoints;
    public Transform face;
    public int num;
    public float GravityMultiplier;

    //DEPRECATED
    public Transform[] ReturnArrayFromID(int id)
    {
        return powerUpSpawnPoints;
    }
}

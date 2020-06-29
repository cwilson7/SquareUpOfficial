using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector] public Transform[] spawnPoints, powerUpSpawnPoints;
    public Transform face;
    public int num;
    public float GravityMultiplier;

    private void Start()
    {
        spawnPoints = ArrayOfChildren(transform.FindChild("PlayerSpawnPoints"));
        powerUpSpawnPoints = ArrayOfChildren(transform.FindChild("PowerUpSpawnPoints"));
    }

    private Transform[] ArrayOfChildren(Transform kid)
    {
        List<Transform> retList = new List<Transform>();
        for (int i = 0; i < kid.childCount; i++)
        {
            retList.Add(kid.GetChild(i));
        }
        Transform[] retArray = retList.ToArray();
        return retArray;
    }

    //DEPRECATED
    public Transform[] ReturnArrayFromID(int id)
    {
        return powerUpSpawnPoints;
    }
}

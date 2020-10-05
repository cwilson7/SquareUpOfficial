using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("TAG IMPORTED 3D OBJECT AS levelObject")]
    [HideInInspector] public Transform[] spawnPoints, powerUpSpawnPoints;
    public GameObject levelModel;
    public Transform face;
    public int num;
    public float GravityMultiplier;
    public AudioClip theme;

    private void Start()
    {
        spawnPoints = ArrayOfChildren(transform.Find("PlayerSpawnPoints"));
        powerUpSpawnPoints = ArrayOfChildren(transform.Find("PowerUpSpawnPoints"));
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

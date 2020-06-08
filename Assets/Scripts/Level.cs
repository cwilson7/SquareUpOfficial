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

    public Level Clone()
    {
        GameObject newLevelGO = Instantiate(gameObject, transform.position, transform.rotation);
        Level newLevel = newLevelGO.GetComponent<Level>();//new Level();
        /*
        newLevel.spawnPoints = spawnPoints;
        newLevel.weaponSpawnPoints = weaponSpawnPoints;
        newLevel.otherPowerUpSpawnPoints = otherPowerUpSpawnPoints;
        GameObject newFace = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), face.position, face.rotation);
        newLevel.face = newFace.transform;
        */
        return newLevel;
    }
}

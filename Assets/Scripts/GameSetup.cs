using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup gs;
    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if (GameSetup.gs == null)
        {
            GameSetup.gs = this;
        }
    }

}

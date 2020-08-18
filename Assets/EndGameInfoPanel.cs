using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameInfoPanel : MonoBehaviour
{
    [SerializeField] GameObject playerList, playerInfoPanel;
    GameObject infoPrefab;

    private void Start()
    {
        infoPrefab = (GameObject)Resources.Load("PhotonPrefabs/EndGame/PlayerInfoGrouping");
    }
}

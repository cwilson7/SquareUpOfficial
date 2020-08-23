using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class EndGameInfoPanel : MonoBehaviour
{
    [SerializeField] GameObject playerList, playerInfoPanel;
    GameObject infoPrefab, infoBtnPrefab;
    Dictionary<GameObject, GameObject> buttonPairs;

    private void Awake()
    {
        infoPrefab = (GameObject)Resources.Load("PhotonPrefabs/EndGame/PlayerInfoGrouping");
        infoBtnPrefab = (GameObject)Resources.Load("PhotonPrefabs/EndGame/PlayerSelectBtn");
    }

    public void InstantiateStats()
    {
        buttonPairs = new Dictionary<GameObject, GameObject>();
        int bestActor = GameInfo.GI.WinningActorNumber();
        foreach (KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject pnl = Instantiate(infoPrefab, playerInfoPanel.transform);
            EndGameInfoGrouping grouping = pnl.GetComponent<EndGameInfoGrouping>();
            grouping.CreateDataPoints(kvp.Key);
            if (grouping.actorNumber == bestActor) pnl.SetActive(true);
            else pnl.SetActive(false);

            GameObject btn = Instantiate(infoBtnPrefab, playerList.transform);
            btn.GetComponent<Button>().onClick.AddListener(delegate { SwitchDisplayedInfo(kvp.Key); });
            btn.GetComponentInChildren<TMP_Text>().text = kvp.Value.NickName;

            buttonPairs.Add(btn, pnl);
        }
    }

    void SwitchDisplayedInfo(int actorNr)
    {
        foreach (KeyValuePair<GameObject, GameObject> kvp in buttonPairs)
        {
            if (kvp.Value.GetComponent<EndGameInfoGrouping>().actorNumber != actorNr)
            {
                kvp.Value.SetActive(false);
            }
            else
            {
                kvp.Value.SetActive(true);
            }
        }
    }
}

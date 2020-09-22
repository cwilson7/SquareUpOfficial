using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class EndGameInfoPanel : MonoBehaviour
{
    int cashPrize = 25;
    
    [SerializeField] GameObject playerList, playerInfoPanel;
    GameObject infoPrefab, infoBtnPrefab;
    Dictionary<GameObject, GameObject> buttonPairs;

    bool playerRewarded = false;

    public Transform characterLocation;

    bool charInit = false;

    private void Awake()
    {
        infoPrefab = (GameObject)Resources.Load("PhotonPrefabs/EndGame/PlayerInfoGrouping");
        infoBtnPrefab = (GameObject)Resources.Load("PhotonPrefabs/EndGame/PlayerSelectBtn");
    }

    public void InstantiateStats()
    {
        buttonPairs = new Dictionary<GameObject, GameObject>();
        int bestActor = GameInfo.GI.WinningActorNumber();
        DisplayActor(bestActor, true);
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
        if (!playerRewarded)
        {
            if (bestActor == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                ProgressionSystem.playerData.Wins += 1;
                PlayerRewards.AddCurrency(ProgressionSystem.playerData, Money.SquareBucks, cashPrize * 2);
            }
            else
            {
                PlayerRewards.AddCurrency(ProgressionSystem.playerData, Money.SquareBucks, cashPrize);
            }
            ProgressionSystem.playerData.totalGames += 1;
            ProgressionSystem.SaveData();
            playerRewarded = true;
        }
    }

    void DisplayActor(int actorNr, bool toDisplay)
    {
        GameObject avatar = GameInfo.GI.avatarClones[actorNr];
        Material mat = LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]];
        avatar.transform.position = characterLocation.position;
        avatar.transform.rotation = Quaternion.Euler(0, 180, 0);
        if (charInit == false)
        {
            AvatarCharacteristics AC = avatar.GetComponent<AvatarCharacteristics>();
            AC.SpawnDummyFists();
            AC.SetFistMaterial(AC.lFist, mat.color);
            AC.SetFistMaterial(AC.rFist, mat.color);
            AC.lFist.GetComponent<Rigidbody>().isKinematic = true;
            AC.rFist.GetComponent<Rigidbody>().isKinematic = true;
        }
        avatar.SetActive(toDisplay);
    }

    void SwitchDisplayedInfo(int actorNr)
    {
        foreach (KeyValuePair<GameObject, GameObject> kvp in buttonPairs)
        {
            int thisActor = kvp.Value.GetComponent<EndGameInfoGrouping>().actorNumber;
            if (thisActor != actorNr)
            {
                DisplayActor(thisActor, false);
                kvp.Value.SetActive(false);
            }
            else
            {
                kvp.Value.SetActive(true);
                DisplayActor(thisActor, true);
            }
        }
    }
}

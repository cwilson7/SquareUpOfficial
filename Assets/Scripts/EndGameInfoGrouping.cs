using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class EndGameInfoGrouping : MonoBehaviour
{
    [SerializeField] TMP_Text userName, avatarName;
    [SerializeField] VerticalLayoutGroup dataPoints;
    [SerializeField] GameObject dataPointPrefab;
    public int actorNumber;

    public void CreateDataPoints(int actorNr)
    {
        actorNumber = actorNr;
        userName.text = PhotonNetwork.CurrentRoom.GetPlayer(actorNr).NickName;
        Score myStats = (Score)GameInfo.GI.scoreTable[actorNr];
        avatarName.text = myStats.playerAvatar.GetComponentInChildren<AvatarCharacteristics>().info.characterName;
        Dictionary<Stat, int> stats = myStats.playerStatistics;
        foreach (KeyValuePair<Stat, int> stat in stats)
        {
            AddDataPoint(System.Enum.GetName(typeof(Stat), stat.Key), stat.Value);
        }
        TMP_Text[] allText = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text txt in allText)
        {
            txt.color = LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]].color;
        }
    }

    GameObject AddDataPoint(string label, float data)
    {
        GameObject txt = Instantiate(dataPointPrefab, dataPoints.gameObject.transform);
        TMP_Text dataPT = txt.GetComponent<TMP_Text>();
        dataPT.text = label + " : " + data;
        return txt;
    }
}

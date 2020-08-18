using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGameInfoGrouping : MonoBehaviour
{
    [SerializeField] TMP_Text userName, avatarName;
    [SerializeField] VerticalLayoutGroup dataPoints;
    GameObject dataPointPrefab;
    int actorNumber;

    public void CreateDataPoints(int actorNr)
    {
        actorNumber = actorNr;
    }

    GameObject AddDataPoint(string label, float data)
    {
        GameObject txt = Instantiate(dataPointPrefab, dataPoints.gameObject.transform);
        TMP_Text dataPT = txt.GetComponent<TMP_Text>();
        dataPT.text = label + " : " + data;
        return txt;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyText : MonoBehaviour
{
    TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (ProgressionSystem.playerData == null) return;
        text.text = "SQUARE BUCKS: " + ProgressionSystem.playerData.SquareBucks.ToString() + "CUBE COINS: " + ProgressionSystem.playerData.CubeCoins.ToString();
    }
}

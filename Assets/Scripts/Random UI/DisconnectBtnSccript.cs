using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisconnectBtnSccript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Disconnect);
        GetComponentInChildren<TMP_Text>().font = MultiplayerSettings.multiplayerSettings.font;
    }

    void Disconnect()
    {
        LobbyController.lc.ReturnToMenu();
    }
}

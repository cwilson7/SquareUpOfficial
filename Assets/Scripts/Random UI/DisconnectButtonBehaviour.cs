using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Disconnect);
    }

    // Update is called once per frame
    private void Disconnect()
    {
        LobbyController.lc.ReturnToMenu();
    }
}

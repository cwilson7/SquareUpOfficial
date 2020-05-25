using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Begin);
    }

    // Update is called once per frame
    private void Begin()
    {
        LobbyController.lc.StartGame();
    }
}

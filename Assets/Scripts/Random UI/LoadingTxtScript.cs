using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingTxtScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TMP_Text>().font = MultiplayerSettings.multiplayerSettings.font;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

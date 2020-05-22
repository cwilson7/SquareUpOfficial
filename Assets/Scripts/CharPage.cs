using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharPage : MonoBehaviour
{
    [SerializeField] private TMP_Text charName;
    
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDetails(int charID)
    {
        charName.text = LobbyController.lc.charAvatars[charID].name;
    }
}

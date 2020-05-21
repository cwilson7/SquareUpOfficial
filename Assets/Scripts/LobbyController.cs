using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyController : MonoBehaviour
{
    public static LobbyController lc;
    
    public List<GameObject> charAvatars;
    
    // Start is called before the first frame update
    void Awake()
    {
        lc = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(PhotonRoom.room.multiplayerScene);
    }
}

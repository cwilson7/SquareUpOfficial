using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyController : MonoBehaviour
{
    public static LobbyController lc;
    
    public List<GameObject> charAvatars;
    public List<Material> availableMaterials;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (LobbyController.lc == null)
        {
            LobbyController.lc = this;
        }
        else
        {
            if (LobbyController.lc != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
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

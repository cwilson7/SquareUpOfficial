using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public int myActorNumber;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        myActorNumber = PV.OwnerActorNr;
        if(PV.IsMine) InitializePhotonPlayer();
    }

    public void Spawn()
    {
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerTestAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
    }

    void Update()
    {

    }

    private void InitializePhotonPlayer()
    {
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerTestAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
    }
}

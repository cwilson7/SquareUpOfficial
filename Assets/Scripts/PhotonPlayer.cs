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
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myActorNumber = PV.OwnerActorNr;
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient) SetUpCube();
            InitializePhotonPlayer();
            //GameInfo.GI.InitializeGameInfo();
        }
    }

    public void SetUpCube()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CubeStuff", "LevelCube"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        Cube.cb.InitializeCube();
    }

    void Update()
    {

    }

    private void InitializePhotonPlayer()
    {
        if (Cube.cb == null) StartCoroutine(InformationDelay());
        else
        {
            Transform[] spawnList = Cube.cb.CurrentFace.spawnPoints;
            int spawnPicker = Random.Range(0, spawnList.Length);
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerTestAvatar"), spawnList[spawnPicker].position, Quaternion.identity/*spawnList[spawnPicker].rotation*/, 0);
        }
    }

    IEnumerator InformationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        InitializePhotonPlayer();
    }
}

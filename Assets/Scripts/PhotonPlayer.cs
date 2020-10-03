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
    public bool makingCubeClone = false;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myActorNumber = PV.OwnerActorNr;
        if (PV.IsMine)
        {
            if (PhotonNetwork.IsMasterClient) SetUpCube();
            InitializePhotonPlayer();
        }
    }

    public void SetUpCube()
    {
        if (!makingCubeClone)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CubeStuff", "LevelCube"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            Cube.cb.InitializeCube();
        }
        else
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (GameInfo.GI.CubeClone.inRotation) PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CubeStuff", "LevelCube"), new Vector3(0f, 0f, GameInfo.GI.CubeClone.DistanceFromCameraForRotation), Quaternion.identity);
            else PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CubeStuff", "LevelCube"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            Cube.cb.DeployClone();
        }
    }

    void Update()
    {
        if (!GameManager.Manager.gameStarted || !PV.IsMine) return;
    }

    private void InitializePhotonPlayer()
    {
        if (Cube.cb == null) StartCoroutine(InformationDelay());
        if (Cube.cb.CurrentFace.spawnPoints.Length < 1) StartCoroutine(InformationDelay());
        else
        {
            Transform[] spawnList = Cube.cb.CurrentFace.spawnPoints;
            Debug.Log(spawnList.Length);
            int spawnPicker = Random.Range(0, spawnList.Length);
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerTestAvatar"), spawnList[spawnPicker].position, spawnList[spawnPicker].rotation, 0);
        }
    }

    IEnumerator InformationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        InitializePhotonPlayer();
    }
}

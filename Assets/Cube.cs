using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Cube : MonoBehaviour
{
    public static Cube cb;
    public PhotonView PV;
    public List<Level> LevelPool = new List<Level>(), LevelsOnCube = new List<Level>();
    public List<int> InstantiatedLevelIDs = new List<int>();
    public List<Transform> Faces = new List<Transform>();
    public Level CurrentFace;

    [SerializeField] private float cubeSize;

    private void Awake()
    {
        cb = this;
        PV = GetComponent<PhotonView>();
    }

    public void InitializeCube()
    {
        PV.RPC("InitializeCube_RPC", RpcTarget.AllBuffered);
        PopulateFaceList();
        SelectAndDeployRandomLevels();
        PV.RPC("SetFirstFace_RPC", RpcTarget.AllBuffered);
    }

    void PopulateFaceList()
    {
        PV.RPC("SetFaces_RPC", RpcTarget.AllBuffered);
    }

    void SelectAndDeployRandomLevels()
    {
        for (int i = 0; i < Faces.Count; i++)
        {
            int id = GenerateRandomLevelID();
            PV.RPC("SetLevels_RPC", RpcTarget.AllBuffered, id, i);
        }
    }

    private int GenerateRandomLevelID()
    {
        int max = LevelPool.Count;
        int id = Random.Range(0, max);
        if (InstantiatedLevelIDs.Contains(id))
        {
            return GenerateRandomLevelID();
        }
        else
        {
            InstantiatedLevelIDs.Add(id);
            return id;
        }
    }
    #region Photon Garbage
    void SetCustomViewID(int id)
    {
        if (PhotonNetwork.AllocateViewID(PV))
        {
            object[] data = new object[]
            {
            transform.position, transform.rotation, PV.ViewID = id
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(69, data, raiseEventOptions, sendOptions);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 69)
        {
            object[] data = (object[])photonEvent.CustomData;

            GameObject cube = (GameObject)Instantiate(Resources.Load<GameObject>("PhotonPrefabs/LevelCube"), (Vector3)data[0], (Quaternion)data[1]);
            PhotonView PV = cube.GetComponent<PhotonView>();
            PV.ViewID = (int)data[2];
        }
    }
#endregion

    [PunRPC]
    public void SetLevels_RPC(int id, int i)
    {
        GameObject level = Instantiate(LevelPool[id].gameObject, Faces[i].transform.position, Faces[i].transform.rotation);
        level.transform.SetParent(transform);
        level.GetComponent<Level>().face = Faces[i];
        LevelsOnCube.Add(level.GetComponent<Level>());
    }
    
    [PunRPC]
    public void SetFirstFace_RPC()
    {
        foreach (Level level in LevelsOnCube)
        {
            if (level.face == Faces[0])
            {
                CurrentFace = level;
            }
        }
    }
    
    [PunRPC]
    public void InitializeCube_RPC()
    {
        cb = this;
        PV = GetComponent<PhotonView>();

        cubeSize = transform.localScale.x;

        Object[] prefabs = Resources.LoadAll("PhotonPrefabs/Levels");
        foreach (Object prefab in prefabs)
        {
            GameObject prefabGO = (GameObject)prefab;
            LevelPool.Add(prefabGO.GetComponent<Level>());
        }
    }
    
    [PunRPC]
    public void SetFaces_RPC()
    {
        //front face Faces[0]
        GameObject faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.forward * cubeSize / 2, Quaternion.Euler(new Vector3(0, 180, 0)));
        Faces.Add(faceLoc.transform);
        //back face Faces[1]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.forward * cubeSize / 2, Quaternion.Euler(new Vector3(0, 0, 0)));
        Faces.Add(faceLoc.transform);
        //right face Faces[2]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, 90, 0)));
        Faces.Add(faceLoc.transform);
        //left face Faces[3]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, -90, 0)));
        Faces.Add(faceLoc.transform);
        //top face Faces[4]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(90, 0, 0)));
        Faces.Add(faceLoc.transform);
        //bottom face Faces[5]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(-90, 0, 0)));
        Faces.Add(faceLoc.transform);

        foreach (Transform face in Faces)
        {
            face.SetParent(transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

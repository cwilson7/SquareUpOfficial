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
    public bool inRotation = false;
    public Quaternion cubeRot;
    public int DistanceFromCameraForRotation = 65;

    private Vector2 targetXY, actualXY;

    [SerializeField] private float cubeSize;

    private void Awake()
    {
        cb = this;
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (inRotation && PV.IsMine) RotateHandler();
    }

    #region Cube Rotation
    private void RotateHandler()
    {
        gameObject.transform.rotation = cubeRot;

        targetXY = new Vector2(rubberBandX(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
                                rubberBandY(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        Vector2 diff = targetXY - actualXY;
        if (diff.magnitude < 0.1)
        {
            actualXY = targetXY;
        }
        else
        {
            actualXY += diff.normalized * 0.05f;
        }

        gameObject.transform.Rotate(transform.InverseTransformVector(Vector3.up), actualXY.x * 90);
        gameObject.transform.Rotate(transform.InverseTransformVector(Vector3.left), actualXY.y * 90);
        if (!PV.IsMine) PV.RPC("SendRotateInformation_RPC", RpcTarget.AllBuffered, actualXY);

        if (Input.GetMouseButtonDown(0) && actualXY.x == Mathf.Floor(actualXY.x) && actualXY.y == Mathf.Floor(actualXY.y))
        {
            cubeRot = gameObject.transform.rotation;
            targetXY = new Vector2(0, 0);
            actualXY = new Vector2(0, 0);
            StopRotation();
        }
    }

    public void StartRotation(int actorNr)
    {
        if (PhotonNetwork.CurrentRoom.GetPlayer(actorNr) == null) return;
        PV.TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(actorNr));
        GameInfo.GI.StopTime();
        inRotation = true;
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, DistanceFromCameraForRotation);
    }

    void StopRotation()
    {
        Vector3 tmpClosest = CurrentFace.face.position;
        for (int i = 0; i < LevelsOnCube.Count; i++)
        {
            if (Vector3.Distance(LevelsOnCube[i].face.position, Camera.main.transform.position) < Vector3.Distance(tmpClosest, Camera.main.transform.position)) tmpClosest = LevelsOnCube[i].face.position;
        }
        PV.RPC("SetFace_RPC", RpcTarget.AllBuffered, tmpClosest);
        GameInfo.GI.StartTime();
        inRotation = false;
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, -DistanceFromCameraForRotation);
    }

    float rubberBandX(float x, float y)
    {
        if (x > 0.85)
        {
            return 1;
        }
        if (x < -0.85)
        {
            return -1;
        }
        if ((y > 0.65 || y < -0.65) && x < 0.3 && x > -0.3)
        {
            return 0;
        }
        return x;
    }

    float rubberBandY(float x, float y)
    {
        if (y > 0.85)
        {
            return 1;
        }
        if (y < -0.85)
        {
            return -1;
        }
        if ((x > 0.65 || x < -0.65) && y < 0.3 && y > -0.3)
        {
            return 0;
        }
        return y;
    }

    #endregion

    #region Cube Setup

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

    #endregion

    #region RPCs
    [PunRPC]
    public void SendRotateInformation_RPC(Vector2 aXY)
    {
        actualXY = aXY;
    }
    
    [PunRPC]
    public void SetLevels_RPC(int id, int i)
    {
        GameObject level = Instantiate(LevelPool[id].gameObject, Faces[i].transform.position, Faces[i].transform.rotation);
        level.GetComponent<Level>().num = i;
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
    public void SetFace_RPC(Vector3 pos)
    {
        foreach (Level level in LevelsOnCube)
        {
            if (level.face.position == pos) CurrentFace = level;
        }
    }
    
    [PunRPC]
    public void InitializeCube_RPC()
    {
        cb = this;
        PV = GetComponent<PhotonView>();
        MiniMapCamera.mmCamera.InitializeMiniMapCamera();

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
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.forward * cubeSize / 2, Quaternion.Euler(new Vector3(0, 180 + 180, 0)));
        Faces.Add(faceLoc.transform);
        //right face Faces[2]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, 90, 0)));
        Faces.Add(faceLoc.transform);
        //left face Faces[3]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, -90, 0)));
        Faces.Add(faceLoc.transform);
        //top face Faces[4]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(180 + 90, 180, 0)));
        Faces.Add(faceLoc.transform);
        //bottom face Faces[5]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(180 - 90, 180, 0)));
        Faces.Add(faceLoc.transform);

        foreach (Transform face in Faces)
        {
            face.SetParent(transform);
        }
    }
    #endregion
}

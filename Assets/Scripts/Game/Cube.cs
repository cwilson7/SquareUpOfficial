using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;

public class Cube : MonoBehaviour, IPunObservable
{
    //FOR TESTING
    public bool testing;
    public GameObject TestingLevel;
    
    public static Cube cb;
    public PhotonView PV;
    public List<Level> LevelPool = new List<Level>(), LevelsOnCube = new List<Level>();
    public List<int> InstantiatedLevelIDs = new List<int>();
    public List<Transform> Faces = new List<Transform>();
    public Level CurrentFace;
    public int currFaceID;
    public bool inRotation = false;
    public Quaternion cubeRot;
    public int DistanceFromCameraForRotation = 65;
    public int ownerActorNr;

    private Vector2 targetXY, actualXY;

    public float cubeSize;

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
        GameManager.Manager.DestroyAllPowerUps();
        ownerActorNr = actorNr;
        GameInfo.GI.StopTime();
        inRotation = true;
        PV.RPC("SendRotateInformation_RPC", RpcTarget.AllBuffered, inRotation, ownerActorNr);
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, DistanceFromCameraForRotation);
    }

    void StopRotation()
    {
        SetClosestFace();
        GameInfo.GI.StartTime();
        inRotation = false;
        PV.RPC("SendRotateInformation_RPC", RpcTarget.AllBuffered, inRotation, ownerActorNr);
        gameObject.transform.position = new Vector3(0f, 0f, 0f);//gameObject.transform.position + new Vector3(0, 0, -DistanceFromCameraForRotation);
    }

    void SetClosestFace()
    {
        Vector3 tmpClosest = CurrentFace.face.position;
        for (int i = 0; i < LevelsOnCube.Count; i++)
        {
            if (Vector3.Distance(LevelsOnCube[i].face.position, Camera.main.transform.position) < Vector3.Distance(tmpClosest, Camera.main.transform.position)) tmpClosest = LevelsOnCube[i].face.position;
        }
        PV.RPC("SetFace_RPC", RpcTarget.AllBuffered, tmpClosest);
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
        PV.RPC("SetFace_RPC", RpcTarget.AllBuffered, Faces[0].position);
    }


    public void DeployClone()
    {
        PV.RPC("InitializeCube_RPC", RpcTarget.AllBuffered);
        PopulateFaceList();
        PV.RPC("SwitchToCubeClone_RPC", RpcTarget.AllBuffered);
        DeploySelectedLevels();
        PV.RPC("SetFace_RPC", RpcTarget.AllBuffered, Faces[currFaceID].position);
        transform.rotation = cubeRot;
        Debug.Log("num of actors left: " + LobbyController.lc.IDsOfDisconnectedPlayers.Count);
        foreach (int i in LobbyController.lc.IDsOfDisconnectedPlayers) Debug.Log("actor left: " + i);
        if (inRotation)
        {
            Player owner = PhotonNetwork.CurrentRoom.GetPlayer(ownerActorNr);
            Debug.Log("current cube owner: " + ownerActorNr);
            if (owner == null)
            {
                inRotation = false;
                ownerActorNr = PV.OwnerActorNr;
                float[] nums = { -270, -180, -90, 0, 90, 180, 270 };
                Vector3 angles = new Vector3(SnapToValue(transform.eulerAngles.x, nums), SnapToValue(transform.eulerAngles.y, nums), SnapToValue(transform.eulerAngles.z, nums));
                cubeRot.eulerAngles = angles;
                transform.localEulerAngles = angles;
                transform.position = new Vector3(0f, 0f, 0f);
                SetClosestFace();
            }
            else PV.TransferOwnership(owner);
        }
    }

    IEnumerator InfoDelay()
    {
        yield return new WaitForSeconds(1f);
    }

    private float SnapToValue(float val, float[] nums)
    {
        float closest = nums[0];
        foreach (float num in nums)
        {
            float newDiff = Mathf.Abs(val - num);
            float smallestDiff = Mathf.Abs(val - closest);
            if (newDiff < smallestDiff)
            {
                closest = num;
            }
        }
        return closest;
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

    void DeploySelectedLevels()
    {
            for (int i = 0; i < InstantiatedLevelIDs.Count; i++)
            {
                int id = InstantiatedLevelIDs[i];
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

    private void AttachToAllChildren<T>(Transform root, LayerMask layer) where T : Component
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.childCount > 0) AttachToAllChildren<T>(child, layer);
            if (child.gameObject.layer == Mathf.RoundToInt(Mathf.Log(layer.value, 2))) child.gameObject.AddComponent<T>();
        }
    }

    #endregion

    #region Paint
    //take paint gameobjects and turn it into data that can be sent over a network
    //PAINT SHOULD BE MOVED TO A CONTAINER OUTSIDE OF CUBE IN HIERARCHY
    System.Collections.Hashtable TranslatePaint()
    {
        System.Collections.Hashtable retHash = new System.Collections.Hashtable();
        PaintInfo[] existingPaintObjs = GetComponentInChildren<PaintObjects>().gameObject.GetComponentsInChildren<PaintInfo>();
        for (int i = 0; i < existingPaintObjs.Length; i++) {
            (int ,Vector3, Vector3) dumbTransform = (existingPaintObjs[i].ID, existingPaintObjs[i].myTransform.position, existingPaintObjs[i].myTransform.eulerAngles);
            retHash.Add(i, dumbTransform);
        }
        return retHash;
    }

    #endregion

    #region RPCs
    [PunRPC]
    public void SwitchToCubeClone_RPC()
    {
        InstantiatedLevelIDs = GameInfo.GI.CubeClone.InstantiatedLevelIDs;
        DistanceFromCameraForRotation = GameInfo.GI.CubeClone.DistanceFromCameraForRotation;
        inRotation = GameInfo.GI.CubeClone.inRotation;
        cubeRot = GameInfo.GI.CubeClone.cubeRot;
        currFaceID = GameInfo.GI.CubeClone.currFaceID;
        ownerActorNr = GameInfo.GI.CubeClone.ownerActorNr;
        GameInfo.GI.CopyCube(cb);
    }
    
    [PunRPC]
    public void SendRotateInformation_RPC(bool inRotation, int ownerNr)
    {
        GameInfo.GI.CubeClone.inRotation = inRotation;
        GameInfo.GI.CubeClone.ownerActorNr = ownerNr;
    }
    
    [PunRPC]
    public void SetLevels_RPC(int id, int i)
    {
        if (!InstantiatedLevelIDs.Contains(id)) InstantiatedLevelIDs.Add(id);
        GameObject level = Instantiate(LevelPool[id].gameObject, Faces[i].transform.position, Faces[i].transform.rotation);
        level.transform.SetParent(Faces[i]);
        level.GetComponent<Level>().num = i;
        level.transform.SetParent(transform);
        level.GetComponent<Level>().face = Faces[i];
        AttachToAllChildren<CollideListener>(level.transform, LayerMask.GetMask("Platform"));
        LevelsOnCube.Add(level.GetComponent<Level>());
    }

    [PunRPC] 
    public void SetFace_RPC(Vector3 pos)
    {
        foreach (Level level in LevelsOnCube)
        {
            if (level.face.position == pos)
            {
                CurrentFace = level;
                currFaceID = level.num;
                if (GameInfo.GI.cubeCloned) GameInfo.GI.CubeClone.currFaceID = level.num;
            }
        }
    }


    [PunRPC]
    public void InitializeCube_RPC()
    {
        cb = this;
        PV = GetComponent<PhotonView>();
        gameObject.AddComponent<CollideListener>();
        MiniMapCamera.mmCamera.InitializeMiniMapCamera();

        cubeSize = transform.localScale.x;

        if (!testing) Utils.PopulateList<Level>(LevelPool, "PhotonPrefabs/Levels");
        else for (int i = 0; i < 9; i++) LevelPool.Add(TestingLevel.GetComponent<Level>());

        if (GameInfo.GI.cubeCloned)
        {
            Score playerInfo = (Score)GameInfo.GI.scoreTable[PV.OwnerActorNr];
            if (!playerInfo.photonPlayer.GetComponent<PhotonPlayer>().makingCubeClone) ownerActorNr = PV.OwnerActorNr;
        }
        else ownerActorNr = PV.OwnerActorNr;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            if (GameInfo.GI.cubeCloned)
            {
                GameInfo.GI.CubeClone.cubeRot = (Quaternion)stream.ReceiveNext();
            }
        }
    }
    #endregion
}

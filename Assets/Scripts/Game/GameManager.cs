using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PhotonView PV;
    public static GameManager Manager;
    public TMP_Text gameTimer;
    
    //Set values
    [SerializeField] private double percentOfPowerUpsWeapons;
    [SerializeField] private float powerUpMaxCooldown;
    [SerializeField] private int maxPowerups;

    //Tracked values
    [SerializeField] private float powerUpCooldown;
    [SerializeField] public bool gameStarted = false, timerRunning = false;
    [SerializeField] private Hashtable currentPowerUps = new Hashtable();

    float maxTimeSeconds = 60f * 3f, timerSeconds; 
    private System.Random rand;
    
    // Start is called before the first frame update
    void Awake()
    {
        Manager = this;
        timerSeconds = maxTimeSeconds;
        SetTimer(maxTimeSeconds);
        PV = GetComponent<PhotonView>();
        rand = new System.Random();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameStarted) return;

        HandleTimers();
        HandlePowerUps();
    }

    public void InitalizeGameManager()
    {
        //Set Initial Values
        powerUpCooldown = powerUpMaxCooldown;
        PV.RPC("GameStart_RPC", RpcTarget.AllBuffered);
    }

    private void HandleTimers()
    {
        if (timerRunning)
        {
            SetTimer(timerSeconds);
            timerSeconds -= Time.deltaTime;
        }
        if (Cube.cb.CurrentFace.powerUpSpawnPoints != null) maxPowerups = Cube.cb.CurrentFace.powerUpSpawnPoints.Length;
        if (currentPowerUps.Count >= maxPowerups || GameInfo.GI.TimeStopped) return;
        if (powerUpCooldown >= 0) powerUpCooldown -= Time.deltaTime;
    }

    void SetTimer(float totalTime)
    {
        int min = Mathf.FloorToInt(totalTime / 60);
        int sec = Mathf.FloorToInt(totalTime % 60f);
        if (sec < 10) gameTimer.text = min + ":0" + sec;
        else gameTimer.text = min + ":" + sec;
    }

    #region Power Up Stuff
    private void HandlePowerUps()
    {
        if (powerUpCooldown < 0f)
        {
            powerUpCooldown = powerUpMaxCooldown;
            if (!PhotonNetwork.IsMasterClient) return;
            if (RandomPercent() <= percentOfPowerUpsWeapons)
            {
                //instantiate weapon power up
                PowerUpSelection(1);
            }
            else
            {
                //instantiate power up
                PowerUpSelection(2);
            }
        }
    }

    private void PowerUpSelection(int idOfList)
    {
        List<GameObject> pwrUpList = GameInfo.GI.ReturnListFromID(idOfList);
        Transform[] pwrUpLocs = Cube.cb.CurrentFace.powerUpSpawnPoints;
        int newID = GenerateIDForPowerUp();
        int locID = GenerateUniqueLocationID(pwrUpLocs);
        //if locid has something generate new random number
        int itemID = RandomInteger(0, pwrUpList.Count);
        PV.RPC("InstantiatePowerUp_RPC", RpcTarget.AllBuffered, idOfList, locID, itemID, newID);
    }

    private int GenerateUniqueLocationID(Transform[] pwrUpLocs)
    {
        int id = RandomInteger(0, pwrUpLocs.Length);
        if (pwrUpLocs[id].childCount > 0) return GenerateUniqueLocationID(pwrUpLocs);
        else return id;
    }

    private int GenerateIDForPowerUp()
    {
        int id = RandomInteger(0, 100);
        if (currentPowerUps.ContainsKey(id))
        {
            return GenerateIDForPowerUp();
        }
        else return id;
    }

    private double RandomPercent()
    {
        return rand.NextDouble();
    }

    private int RandomInteger(int min, int max)
    {
        return rand.Next(max);
    }

    public void DestroyAllPowerUps()
    {
        int keyLength = currentPowerUps.Count;
        List<int> keys = new List<int>();
        foreach (int key in currentPowerUps.Keys) keys.Add(key);
        for (int i = 0; i < keyLength; i++)
        {
            PV.RPC("DestroyPowerUp_RPC", RpcTarget.AllBuffered, keys[i]);
        }
    }

    #endregion

    #region RPCs
    [PunRPC]
    public void InstantiatePowerUp_RPC(int listID, int locID, int pwrUpID, int newID)
    {
        List<GameObject> pwrUpList = GameInfo.GI.ReturnListFromID(listID);
        Transform[] pwrUpLocs = Cube.cb.CurrentFace.powerUpSpawnPoints; //ReturnArrayFromID(listID);
        GameObject pwrUp = Instantiate(pwrUpList[pwrUpID], pwrUpLocs[locID]);
        pwrUp.GetComponent<PowerUp>().id = newID;
        currentPowerUps.Add(newID, pwrUp);
    }

    [PunRPC]
    public void DestroyPowerUp_RPC(int id)
    {
        if (!currentPowerUps.ContainsKey(id)) return;
        
        GameObject pwrUp = (GameObject)currentPowerUps[id];
        Destroy(pwrUp);
        currentPowerUps.Remove(id);
    }

    [PunRPC]
    public void EquipWeapon_RPC(int actor, string weaponName)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actor];
        //GameObject wpn = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + weaponName), playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform.position, Quaternion.identity);
        GameObject wpn = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + weaponName), playerInfo.playerAvatar.GetComponentInChildren<GunLocation>().transform.position, Quaternion.identity);
        wpn.transform.SetParent(playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform);
        //wpn.transform.SetParent(playerInfo.playerAvatar.transform);
        wpn.GetComponent<Weapon>().owner = actor;
        wpn.GetComponent<Weapon>().GunLocation = playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform;
        playerInfo.playerAvatar.GetComponent<Controller>().currentWeapon = wpn.GetComponent<Weapon>();
        playerInfo.playerAvatar.GetComponent<Controller>().EquipWeapon();
        //wpn.transform.rotation = new Vector3()
    }

    [PunRPC]
    public void GameStart_RPC()
    {
        gameStarted = true;
        timerRunning = true;
    }

    #endregion
}

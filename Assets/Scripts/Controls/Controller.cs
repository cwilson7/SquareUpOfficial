using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;
using System.IO;

public abstract class Controller : MonoBehaviour
{
    public bool iPhone = false;

    private PhotonView PV;
    private CharacterController cc;
    [SerializeField] private GameObject baseOfCharacterPrefab;

    public Weapon currentWeapon;
    public Fist Fist;
    public MiniMapPlayer mmPlayer;

    //Control UI
    protected FloatingJoystick moveStick;

    //Initial Player movement variables
    public int actorNr;
    public float speed, gravity, jumpHeightMultiplier, groundDetectionRadius, distanceFromGround;
    public int maxJumps;
    public Transform baseOfCharacter;
    public float punchPower, punchImpact, punchCooldown;
    [SerializeField] float respawnDelay, boundaryDist;

    //Tracked variables
    public Vector3 Velocity, impact;
    public int jumpNum;
    public double HP;
    public Vector3 AimDirection;
    private bool controllerInitialized = false;
    public bool isDead = false;

    public Animator anim;

    #region SET VALUES
    // Start is called before the first frame update
    void Start()
    {
        //InitializePlayerController();
    }

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();

        impact = Vector3.zero;
        AimDirection = Vector2.zero;

        //Default values for all players
        speed = 10f;
        gravity = -9.8f;
        jumpHeightMultiplier = 1f;
        groundDetectionRadius = 0.1f;
        maxJumps = 2;
        distanceFromGround = 0.5f;
        HP = 100;
        punchPower = 10f;
        punchImpact = 1f;
        punchCooldown = 2;
        respawnDelay = 3f;
        boundaryDist = 100f;
        actorNr = GetComponent<PhotonView>().OwnerActorNr;

        cc = GetComponent<CharacterController>();
        currentWeapon = GetComponent<Weapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        if (iPhone) SwipeDetector.OnSwipe += TouchCombat;
        if (!iPhone) moveStick.gameObject.SetActive(false);

        Fist = GetComponentInChildren<Fist>();
        Fist.InitializeFist();

        mmPlayer = GetComponentInChildren<MiniMapPlayer>();

        baseOfCharacter = GetComponentInChildren<BaseOfCharacter>().gameObject.transform;
        baseOfCharacter.transform.position = new Vector3(baseOfCharacter.position.x, baseOfCharacter.position.y - distanceFromGround, baseOfCharacter.transform.position.z);

        anim = GetComponentInChildren<Animator>();

        controllerInitialized = true;
        if (PV.IsMine) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("ControllerInitialized", true);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!controllerInitialized || GameInfo.GI.TimeStopped || isDead) return;
        Gravity();

        if (!PV.IsMine) return;             
        Movement();
        HandleDeaths();
        if (!iPhone)
        {
            TrackMouse();
            MouseCombat();
        }
        SpecialAbility();
    }

    private void MouseCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == null)
            {
                Fist.Smack(AimDirection);
            }
            else
            {
                currentWeapon.Attack(AimDirection);
            }
        }
    }

    public void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = 0f;
        AimDirection = (MouseWorldPos - transform.position).normalized;
    }

    #region Death/ Respawn

    private void HandleDeaths()
    {
        float vertDistance = Mathf.Abs(transform.position.y - Cube.cb.CurrentFace.face.position.y);
        float horizDistance = Mathf.Abs(transform.position.x - Cube.cb.CurrentFace.face.position.x);
        if (horizDistance > boundaryDist || vertDistance > boundaryDist || HP <= 0.0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        GameInfo.GI.StatChange(PhotonNetwork.LocalPlayer.ActorNumber, "deaths");
        //explode with color
        SetAllComponents(false);
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        int locID = UnityEngine.Random.Range(0, list.Length);
        transform.position = list[locID].position;
        transform.rotation = list[locID].rotation;
        SetAllComponents(true);
        isDead = false;
        //spawn effect
    }

    private void SetAllComponents(bool isActive)
    {
        foreach (MeshRenderer display in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            display.enabled = isActive;
        }
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = isActive;
        }
    }

    #endregion

    #region Movement Functions
    public virtual void Gravity()
    {
        LayerMask ground = LayerMask.GetMask("Platform");
        bool isGrounded = Physics.CheckSphere(baseOfCharacter.position, groundDetectionRadius, ground);
        if (isGrounded && Velocity.y < 0)
        {
            Velocity.y = 0f;
            jumpNum = maxJumps;
        }
        else Velocity.y += gravity * Time.deltaTime;
    }

    public virtual void Movement()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (iPhone)
        {
            //Vertical movement
            if (moveStick.Vertical >= 0.8)
            {
                Jump();
            }

            //Horizontal movement
            if (moveStick.Horizontal >= 0.2)
            {
                Debug.Log("Move");
                Velocity.x = 1;
                anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (moveStick.Horizontal <= -0.2)
            {
                Velocity.x = -1;
                anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                Velocity.x = 0;
                anim.SetBool("Run", false);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
            if (Input.GetAxis("Horizontal") > 0)
            {
                anim.SetBool("Running", true);
                gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                anim.SetBool("Running", true);
                gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
            }
            if (Input.GetAxis("Horizontal") == 0)
            {
                anim.SetBool("Running", false);
            }
            Velocity.x = Input.GetAxis("Horizontal");
        }


        Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
    public void Jump()
    {
        if (jumpNum <= 0) return;
        anim.SetTrigger("Jump");
        Velocity.y = Mathf.Sqrt(jumpHeightMultiplier * -1f * gravity);
        jumpNum -= 1;
    }
    #endregion

    #region Touch Functions
    public virtual void TouchCombat(SwipeData data)
    {
        Vector3 AttackDirection = data.StartPos - data.EndPos;
        AimDirection = AttackDirection;
        if (currentWeapon == null)
        {
            Fist.Smack(AimDirection);
        }
        else
        {
            currentWeapon.Attack(AimDirection);
        }
    }
    #endregion

    #region Collision/ Trigger 
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Projectile proj = other.gameObject.GetComponent<Projectile>();
            if (proj.owner == actorNr) return;

            if (PV.IsMine)
            {
                LoseHealth(proj.damage);
                GameInfo.GI.StatChange(proj.owner, "bulletsLanded");
            }
            
            impact += proj.impactMultiplier * proj.Velocity.normalized;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fist")
        {
            Fist fist = other.GetComponent<Fist>();
            if (fist.owner == actorNr) return;

            if (PV.IsMine)
            {
                LoseHealth(fist.damage);
                GameInfo.GI.StatChange(fist.owner, "punchesLanded");
            }

            impact += fist.impact * fist.gameObject.GetComponent<Rigidbody>().velocity.normalized;
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region RPC

    public void LoseHealth(double lostHP)
    {
        PV.RPC("LoseHealth_RPC", RpcTarget.AllBuffered, lostHP);
    }

    [PunRPC]
    public void FireWeapon_RPC(Vector3 Direction, float dmg, float impt, float bltSpd, int ownr, string projName)
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + projName), currentWeapon.FiringPoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().InitializeProjectile(dmg, impt, Direction * (float)bltSpd, ownr);
    }

    [PunRPC]
    public void LoseHealth_RPC(double lostHP)
    {
        HP -= lostHP;
    }

    [PunRPC]
    public void LoseWeapon_RPC()
    {
        if (currentWeapon == null) return;
        currentWeapon.Remove();
        currentWeapon = null;
    }

    [PunRPC]
    public void RPC_MeleAttack(Vector3 aim, int actorNumber)
    {
        aim = new Vector3(aim.x, aim.y, 0f);
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        GameObject fist = playerInfo.playerAvatar.GetComponent<Controller>().Fist.gameObject;
        fist.GetComponent<Fist>().cooldown = Fist.GetComponent<Fist>().timeBtwnPunches;
        fist.GetComponent<Rigidbody>().AddForce(aim * 1000);
        fist.GetComponent<SphereCollider>().enabled = true;
        StartCoroutine(fist.GetComponent<Fist>().FistDrag());
    }

    [PunRPC]
    public void SetUpMiniMap_RPC(int actorNumber)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        MiniMapPlayer player = playerInfo.playerAvatar.GetComponent<Controller>().mmPlayer;
        int colorid = (int)PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties["AssignedColor"];
        player.gameObject.GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[colorid];
    }
    #endregion

    #region Coroutines
    #endregion

    public abstract void SpecialAbility();
}

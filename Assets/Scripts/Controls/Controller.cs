using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;
using System.IO;

public abstract class Controller : MonoBehaviour
{
    public static event Action<DamageDealer, Controller> OnDamgeTaken;
    
    public bool iPhone = false;

    public PhotonView PV;
    public ParticleSystem PaintExplosionSystem;
    public CharacterController cc;
    [SerializeField] private GameObject baseOfCharacterPrefab;
    public BoxCollider _Collider;

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
    public float punchPower, punchImpact, punchCooldown, specialCooldown;
    [SerializeField] float respawnDelay, boundaryDist;
    public float fistActiveTime, fistRadius;

    //Tracked variables
    public Vector3 Velocity, impact;
    public int jumpNum;
    public Healthbar health;
    public Vector3 AimDirection;
    public bool controllerInitialized = false;
    public bool isDead = false;
    public bool isRunning, hasGun;
    public int directionModifier;

    public Animator anim;

    #region SET VALUES

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();
        _Collider = GetComponentInChildren<AvatarCharacteristics>().gameObject.GetComponent<BoxCollider>();

        impact = Vector3.zero;
        AimDirection = Vector2.zero;

        //Default values for all players
        speed = 15f;
        gravity = -9.8f;
        jumpHeightMultiplier = 1f;
        groundDetectionRadius = 0.5f;
        maxJumps = 2;
        distanceFromGround = 0.5f;
        health = GameObject.Find("Healthbar1").GetComponent<Healthbar>();
        //health.maximumHealth = 100;
        //ealth.health = 100;
        //health.m
        punchPower = 10f;
        punchImpact = 1.5f;
        punchCooldown = 1;
        specialCooldown = 1;
        respawnDelay = 3f;
        boundaryDist = 100f;
        fistActiveTime = 0.5f;
        directionModifier = 1;
        fistRadius = 5f;
        _Collider.isTrigger = true;
        _Collider.enabled = true;
        actorNr = GetComponent<PhotonView>().OwnerActorNr;

        PaintExplosionSystem = GetComponentInChildren<ParticleSystem>();
        
        cc = GetComponent<CharacterController>();
        currentWeapon = GetComponent<Weapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        if (iPhone) SwipeDetector.OnSwipe += TouchCombat;
        if (!iPhone) moveStick.gameObject.SetActive(false);

        Fist = GetComponentInChildren<Fist>();
        Fist.InitializeFist(this);

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
        HandleAnimationValues();
        if (!iPhone)
        {
            TrackMouse();
            MouseCombat();
        }
        
    }
    private void FixedUpdate()
    {
        if (specialCooldown >= 0) specialCooldown -= Time.deltaTime;
    }

    private void HandleAnimationValues()
    {
        //set values for aimx and aimy

        if (currentWeapon != null) hasGun = true;
        else if (currentWeapon == null) hasGun = false;

        if (Mathf.Abs(Velocity.x) > 0) isRunning = true;
        else isRunning = false;
    }

    #region Mouse Tracking / Combat
    private void MouseCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == null && Fist.cooldown <= 0)
            {
                Punch();
            }
            else if (currentWeapon != null)
            {
                currentWeapon.Attack(AimDirection);
            }
        }
    }

    public void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = transform.position.z;
        AimDirection = (MouseWorldPos - transform.position).normalized;
        AimDirection.z = transform.position.z;

        anim.SetFloat("AimX", AimDirection.x * directionModifier);
        anim.SetFloat("AimY", AimDirection.y);
    }

    private void Punch()
    {
        GameInfo.GI.StatChange(actorNr, "punchesThrown");
        PV.RPC("RPC_MeleAttack", RpcTarget.AllBuffered, AimDirection, actorNr);
    }

    #endregion

    #region Death/ Respawn

    private void HandleDeaths()
    {
        float vertDistance = Mathf.Abs(transform.position.y - Cube.cb.CurrentFace.face.position.y);
        float horizDistance = Mathf.Abs(transform.position.x - Cube.cb.CurrentFace.face.position.x);
        if (horizDistance > boundaryDist || vertDistance > boundaryDist || health.health <= 0.0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        isDead = true;
        GameInfo.GI.StatChange(PhotonNetwork.LocalPlayer.ActorNumber, "deaths");
        //explode with color
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        int spawnPtlocID = UnityEngine.Random.Range(0, list.Length);
        PV.RPC("DieAndRespawn_RPC", RpcTarget.AllBuffered, spawnPtlocID);
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (GameInfo.GI.TimeStopped) StartCoroutine(CubeRotationWait());
        else Respawn();
    }

    private void Respawn()
    {
        if (PV.IsMine)
        {
            health.health = 100;
            isDead = false;
            Velocity = Vector3.zero;
        }
        SetAllComponents(true);
        //spawn effect
    }

    IEnumerator CubeRotationWait()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameInfo.GI.TimeStopped) StartCoroutine(CubeRotationWait());
        else Respawn();
    }

    private void SetAllComponents(bool isActive)
    {
        foreach (SkinnedMeshRenderer display in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
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
        else Velocity.y += Cube.cb.CurrentFace.GravityMultiplier * gravity * Time.deltaTime;
    }

    public virtual void Movement()
    {

        if (iPhone)
        {
            //Vertical movement
            if (moveStick.Vertical >= 0.8)
            {
                TryJump();
            }

            //Horizontal movement
            if (moveStick.Horizontal >= 0.2)
            {
                Debug.Log("Move");
                Velocity.x = 1;
                //anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (moveStick.Horizontal <= -0.2)
            {
                Velocity.x = -1;
                //anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                Velocity.x = 0;
                //anim.SetBool("Run", false);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryJump();
            }
            if (Input.GetAxis("Horizontal") > 0 || Input.GetKeyDown(KeyCode.D))
            {
                directionModifier = 1;
                gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
                anim.SetBool("Running", true);
            }
            if (Input.GetAxis("Horizontal") < 0 || Input.GetKeyDown(KeyCode.A))
            {
                directionModifier = -1;
                gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
                anim.SetBool("Running", true);
            }
            if (Input.GetAxis("Horizontal") == 0)
            {
                anim.SetBool("Running", false);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TrySpecial();
            }

            Velocity.x = Input.GetAxis("Horizontal");
        }


        Move(Velocity);
        //Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        //cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        Vector3 move = new Vector3(_velocity.x, _velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);
    }

    public void TrySpecial()
    {
        if (specialCooldown <= 0)
        {
            PV.RPC("SpecialAnimation_RPC", RpcTarget.AllBuffered, actorNr);
        }
    }
    public void TryJump()
    {
        if (jumpNum <= 0) return;
        PV.RPC("JumpAnimation_RPC", RpcTarget.AllBuffered, actorNr);
    }

    public void JumpAction()
    {
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
            //animate fist
            //call RPC melee attack
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
        GameObject otherGO = other.gameObject;
        if (otherGO.tag == "Projectile")
        {
            Projectile proj = otherGO.GetComponent<Projectile>();
            if (proj.owner == actorNr) return;

            if (PV.IsMine)
            {
                LoseHealth(proj.damage);
                OnDamgeTaken?.Invoke(proj, this);
                GameInfo.GI.StatChange(proj.owner, "bulletsLanded");
            }
            impact += proj.impactMultiplier * proj.Velocity.normalized;
            Destroy(otherGO);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fist")
        {
            Fist fist = other.GetComponent<Fist>();
            if (fist.owner == actorNr) return;
            fist.SetCollider(false);

            if (PV.IsMine)
            {
                LoseHealth(fist.damage);
                OnDamgeTaken?.Invoke(fist, this);
                GameInfo.GI.StatChange(fist.owner, "punchesLanded");
            }
            impact += fist.impactMultiplier * fist.Velocity.normalized;
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
        Direction.z = 0f;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + projName), currentWeapon.FiringPoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().InitializeProjectile(dmg, impt, Direction * (float)bltSpd, ownr);
    }

    [PunRPC]
    public void DieAndRespawn_RPC(int locID)
    {
        SetAllComponents(false);
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        transform.position = list[locID].position;
        transform.rotation = list[locID].rotation;
        StartCoroutine(SpawnDelay());
    }

    [PunRPC]
    public void LoseHealth_RPC(double lostHP)
    {
        health.health -= (float)lostHP;
    }

    [PunRPC]
    public void LoseWeapon_RPC()
    {
        if (currentWeapon == null) return;
        currentWeapon.Remove();
        anim.SetBool("Gun", false);
        currentWeapon = null;
    }

    [PunRPC]
    public void RPC_MeleAttack(Vector3 aimDir, int actorNumber)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        playerInfo.playerAvatar.GetComponent<Controller>().anim.SetTrigger("Melee");
        playerInfo.playerAvatar.GetComponent<Controller>().Fist.Punch();
    }

    [PunRPC]
    public void SetUpMiniMap_RPC(int actorNumber)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        MiniMapPlayer player = playerInfo.playerAvatar.GetComponent<Controller>().mmPlayer;
        int colorid = (int)PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties["AssignedColor"];
        player.gameObject.GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[colorid];
    }

    [PunRPC]
    public void JumpAnimation_RPC(int id)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[id];
        playerInfo.playerAvatar.GetComponent<Controller>().JumpAction();
    }

    [PunRPC]
    public void SpecialAnimation_RPC(int id)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[id];
        playerInfo.playerAvatar.GetComponent<Controller>().SpecialAbility();
    }

    #endregion

    #region Coroutines
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        BoxCollider _Collider = GetComponent<BoxCollider>();
        if (_Collider == null) return;
        Gizmos.DrawWireCube(_Collider.transform.position + _Collider.center, _Collider.size);
    }

    public abstract void SpecialAbility();
}

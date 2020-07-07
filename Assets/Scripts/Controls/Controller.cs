﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;
using System.IO;
using CustomUtilities;

public abstract class Controller : MonoBehaviour
{
    public static event Action<DamageDealer, Controller> OnDamgeTaken;
    
    public bool iPhone = false;

    public PhotonView PV;
    public ParticleSystem PaintExplosionSystem;
    [SerializeField] private GameObject baseOfCharacterPrefab;
    private Rigidbody rb;
    private SphereCollider GroundCollider;

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
    public float punchPower, punchImpact, punchCooldown, punchCDTime, specialCooldown, specialCDTime;
    [SerializeField] float respawnDelay, boundaryDist;
    public float fistActiveTime, fistRadius;
    private float ogMass;

    //Tracked variables
    public Vector3 Velocity, impact;
    public int jumpNum;
    public float HP;
    public Vector3 AimDirection;
    public bool controllerInitialized = false;
    public bool isDead = false;
    public bool isRunning, hasGun;
    public int directionModifier;
    private bool isGrounded;
    public bool sliding;
    public float rayDist = 1f;
    public float slideLimit = 45;

    public Animator anim;
    public int numOfClicks;
    public float lastClickTime;
    public float maxClickDelay;



    #region SET VALUES

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();

        impact = Vector3.zero;
        AimDirection = Vector2.zero;

        //Default values for all players
        speed = 25f;
        gravity = -9.8f;
        jumpHeightMultiplier = 50f;
        groundDetectionRadius = 0.75f;
        maxJumps = 2;
        distanceFromGround = 0.5f;
        HP = 1f;
        punchPower = 0.1f;
        punchImpact = 1.5f;
        punchCooldown = 1;
        punchCDTime = 0f;
        specialCooldown = 1;
        specialCDTime = 0f;
        respawnDelay = 3f;
        boundaryDist = 100f;
        fistActiveTime = 0.5f;
        directionModifier = 1;
        fistRadius = 5f;
        actorNr = GetComponent<PhotonView>().OwnerActorNr;
        ogMass = rb.mass;
        numOfClicks = 0;
        lastClickTime = 0;
        maxClickDelay = 0.5f;
        isGrounded = false;

        sliding = false;
        rayDist = 1f;
        slideLimit = 45;

        PaintExplosionSystem = GetComponentInChildren<ParticleSystem>();
        
        currentWeapon = GetComponent<Weapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        if (iPhone) SwipeDetector.OnSwipe += TouchCombat;
        if (!iPhone) moveStick.gameObject.SetActive(false);

        Fist = GetComponentInChildren<Fist>();
        Fist.InitializeFist(this);

        mmPlayer = GetComponentInChildren<MiniMapPlayer>();

        baseOfCharacter = GetComponentInChildren<BaseOfCharacter>().gameObject.transform;
        baseOfCharacter.transform.position = new Vector3(baseOfCharacter.position.x, baseOfCharacter.position.y - distanceFromGround, baseOfCharacter.transform.position.z);

        GroundCollider = GetComponentInChildren<GroundColliderBone>().gameObject.GetComponent<SphereCollider>();

        anim = GetComponentInChildren<Animator>();

        //rb.inertiaTensor = rb.inertiaTensor + new Vector3(rb.inertiaTensor.x * 100, rb.inertiaTensor.y * 100, rb.inertiaTensor.z * 100);

        controllerInitialized = true;
        if (PV.IsMine) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("ControllerInitialized", true);


    }

    #endregion

    #region Update Functions

    // OnlyThings which cant go in fixed update
    void Update()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();

        if (!PV.IsMine) return;
        else
        {
            //rayCastChecks();
            Movement();
            HandleDeaths();
            HandleAnimationValues();
            if (!iPhone)
            {
                TrackMouse();
                MouseCombat();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (specialCDTime >= 0) specialCDTime -= Time.deltaTime;
        if (punchCDTime >= 0) punchCDTime -= Time.deltaTime;
        if (rb.velocity.y < 0) rb.velocity += Vector3.up * Physics.gravity.y * 0.5f * Time.deltaTime;
    }

    private bool CheckForTimeStop()
    {
        if (GameInfo.GI.TimeStopped || isDead)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            return true;
        }
        else
        {
            rb.useGravity = true;
            return false;
        }
    }

    private void TrackHP()
    {
        AvatarCharacteristics ColorInfo = GetComponentInChildren<AvatarCharacteristics>();
        ColorInfo.UpdateMaterial(Color.Lerp(LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]].color, Color.black, 1 - HP));
    }

    private void HandleAnimationValues()
    {
        //set values for aimx and aimy

        if (currentWeapon != null) hasGun = true;
        else if (currentWeapon == null) hasGun = false;

        if (Mathf.Abs(Velocity.x) > 0) isRunning = true;
        else isRunning = false;
    }

    #endregion

    #region Mouse Tracking / Combat
    private void MouseCombat()
    {
        if (Time.time - lastClickTime > maxClickDelay && numOfClicks > 0)
        {
            numOfClicks = 0;
            punchCDTime = punchCooldown;
            PV.RPC("RPC_MeleeEnd", RpcTarget.AllBuffered, actorNr, numOfClicks);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == null && punchCDTime <= 0)
            {
                lastClickTime = Time.time;
                numOfClicks++;
                Punch(numOfClicks);
            }
            else if (currentWeapon != null)
            {
                currentWeapon.Attack(AimDirection);
            }
        }
    }

    void Flinch(bool fromLeft)
    {
        int mod;
        if (PV.IsMine) mod = directionModifier;
        else
        {
            mod = GetComponent<AnimationSynchronization>().directionModifier;
            GetComponent<AnimationSynchronization>().SetFlinch();
        }
        if (fromLeft)
        {
            if (mod == 1) anim.SetTrigger("RecoilF");
            else anim.SetTrigger("RecoilB");
        }
        else
        {
            if (mod == 1) anim.SetTrigger("RecoilB");
            else anim.SetTrigger("RecoilF");
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

    private void Punch(int numPunch)
    {
        GameInfo.GI.StatChange(actorNr, "punchesThrown");
        PV.RPC("RPC_MeleeAttack", RpcTarget.AllBuffered, AimDirection, actorNr, numPunch);
    }

    #endregion

    #region Death/ Respawn

    private void HandleDeaths()
    {
        float vertDistance = Mathf.Abs(transform.position.y - Cube.cb.CurrentFace.face.position.y);
        float horizDistance = Mathf.Abs(transform.position.x - Cube.cb.CurrentFace.face.position.x);
        if (horizDistance > boundaryDist || vertDistance > boundaryDist || HP <= 0f)
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
            isDead = false;
            Velocity = Vector3.zero;
        }
        GetComponentInChildren<AvatarCharacteristics>().SetMaterial(LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]]);
        HP = 1f;
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

    public virtual void Movement()
    {
        HandleInputs(iPhone);

        Move(Velocity);
    }

    protected void HandleInputs(bool iPhone)
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
                //Debug.Log("Move");
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
            if (Input.GetAxis("Horizontal") > 0 || Input.GetKeyDown(KeyCode.D))
            {
                FreezePositons(false, false);
                directionModifier = 1;
                gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
                anim.SetBool("Running", true);

            }
            if (Input.GetAxis("Horizontal") < 0 || Input.GetKeyDown(KeyCode.A))
            {
                FreezePositons(false, false);
                directionModifier = -1;
                gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
                anim.SetBool("Running", true);

            }
            if (Input.GetAxis("Horizontal") == 0)
            {
                anim.SetBool("Running", false);
                if (isGrounded)
                {
                    FreezePositons(true, false);
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                FreezePositons(false, false);
                TryJump();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TrySpecial();
            }
            Velocity.x = Input.GetAxis("Horizontal");
        }
    }

    public void Move(Vector3 _velocity)
    {
        rb.velocity = new Vector3(_velocity.x * speed + impact.x, rb.velocity.y + impact.y, 0f);

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
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
        isGrounded = false;
        anim.SetTrigger("Jump");
        //rb.AddForce(jumpHeightMultiplier * Vector3.up);
        rb.velocity = new Vector3(rb.velocity.x, jumpHeightMultiplier, 0f);
        jumpNum -= 1;
    }

    private void FreezePositons(bool x, bool y)
    {
        if (x)
        {
            if (y)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            //rb.useGravity = false;
            
        }
        else
        {
            if (y)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            }
            else rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
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
        GroundCheck(other);
        GameObject otherGO = other.gameObject;
        if (otherGO.tag == "Projectile")
        {
            Projectile proj = otherGO.GetComponent<Projectile>();
            if (proj.owner == actorNr) return;
            bool fromLeft = proj.Velocity.x > 0;

            if (proj.owner == PhotonNetwork.LocalPlayer.ActorNumber) Flinch(fromLeft);

            if (PV.IsMine)
            {
                OnDamgeTaken?.Invoke(proj, this);
                LoseHealth(proj.damage);
                PV.RPC("Flinch_RPC", RpcTarget.AllBuffered, fromLeft);
                GameInfo.GI.StatChange(proj.owner, "bulletsLanded");
            }
            //impact += proj.impactMultiplier * proj.Velocity.normalized;
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
            bool fromLeft = fist.startLoc.x < transform.position.x;

            if (fist.owner == PhotonNetwork.LocalPlayer.ActorNumber) Flinch(fromLeft);

            if (PV.IsMine)
            {
                OnDamgeTaken?.Invoke(fist, this);
                LoseHealth(fist.damage);
                PV.RPC("Flinch_RPC", RpcTarget.AllBuffered, fromLeft);
                GameInfo.GI.StatChange(fist.owner, "punchesLanded");
            }
            
            //impact += fist.impactMultiplier * fist.Velocity.normalized;
        }
    }

    void GroundCheck(Collision collision)
    {
        bool touchingGround = false;
        if (collision.collider.gameObject.layer == 8)
        {
            foreach (ContactPoint cp in collision.contacts)
            {
                if (cp.thisCollider == GroundCollider)
                {
                    touchingGround = true;
                    break;
                }
            }
        }
        isGrounded = touchingGround;
        if (isGrounded) jumpNum = maxJumps;
    }

    #endregion

    #region RPC

    public void LoseHealth(float lostHP)
    {
        PV.RPC("LoseHealth_RPC", RpcTarget.AllBuffered, lostHP);
    }

    [PunRPC]
    public void FireWeapon_RPC(Vector3 Direction, float dmg, float impt, float bltSpd, int ownr, string projName)
    {
        if (currentWeapon.FiringPoint == null) return;
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
        //transform.rotation = list[locID].rotation;
        StartCoroutine(SpawnDelay());
    }

    [PunRPC]
    public void LoseHealth_RPC(float lostHP)
    {
        HP -= lostHP;
    }

    [PunRPC]
    public void Flinch_RPC(bool fromLeft)
    {
        //if i already flinched from this attack, return
        if (GetComponent<AnimationSynchronization>().flinched) GetComponent<AnimationSynchronization>().flinched = false;
        else Flinch(fromLeft);
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
    public void RPC_MeleeAttack(Vector3 aimDir, int actorNumber, int punchNum)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        playerInfo.playerAvatar.GetComponent<Controller>().anim.SetInteger("Melee",punchNum);
        playerInfo.playerAvatar.GetComponent<Controller>().Fist.SetCollider(true);
    }
    [PunRPC]
    public void RPC_MeleeEnd(int actorNumber, int punchNum)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        playerInfo.playerAvatar.GetComponent<Controller>().anim.SetInteger("Melee", punchNum);
        playerInfo.playerAvatar.GetComponent<Controller>().Fist.SetCollider(false);
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

    public abstract void SpecialAbility();
}

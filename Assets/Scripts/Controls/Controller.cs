using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;
using System.IO;
using CustomUtilities;
using Photon.Realtime;

public abstract class Controller : MonoBehaviour
{
    public static event Action<DamageDealer, Controller> OnDamgeTaken;
    public ParticleSystem PaintExplosionSystem;

    public PhotonView PV;
    protected Rigidbody rb;
    protected SphereCollider GroundCollider;
    protected AvatarCharacteristics avatarCharacteristics;
    protected Material myMat;

    public Weapon currentWeapon;
    public Fist RFist; //fist num 1
    public Fist LFist; //fist num 0
    public MiniMapPlayer mmPlayer;

    GameObject deathEffect, spawnEffect;

    //Control UI
    protected FloatingJoystick moveStick;

    //Initial Player movement variables
    public int actorNr;
    public float speed, gravity, jumpHeightMultiplier, distanceFromGround;
    public int maxJumps;
    public Transform baseOfCharacter;
    public float punchPower, punchImpact;
    [SerializeField] float respawnDelay, boundaryDist;

    //Tracked variables
    public Vector3 impact;
    public int jumpNum;
    public float HP;
    public Vector3 AimDirection;
    public bool hasGun, isGrounded, isDead = false, controllerInitialized = false, receivingImpact = false;
    float impactInterp = 0f;
    public int directionModifier;
    public Vector3 tempVel;

    public Animator anim;

    public AudioHandler audioHandler;
    public string audioKey;

    Vector3 respawnPos;
    GameObject crown;

    protected bool abilityOffCooldown = true, unfreezeForAbility = false;


    #region SET VALUES

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();

        rb = GetComponent<Rigidbody>();

        PaintExplosionSystem = GetComponentInChildren<ParticleSystem>();

        avatarCharacteristics = GetComponentInChildren<AvatarCharacteristics>();

        spawnEffect = Resources.Load<GameObject>(avatarCharacteristics.PathOfEffect(EffectType.Ability));
        deathEffect = Resources.Load<GameObject>(avatarCharacteristics.PathOfEffect(EffectType.Ability));

        impact = Vector3.zero;
        AimDirection = Vector2.zero;

        //Default values for all players
        speed = 35f;
        gravity = -9.8f;
        jumpHeightMultiplier = 50f;
        maxJumps = 2;
        distanceFromGround = 0.5f;
        HP = 1f;
        punchPower = 0.1f;
        punchImpact = 0.75f;
        respawnDelay = 3f;
        boundaryDist = 150f;
        directionModifier = 1;
        actorNr = PV.OwnerActorNr;
        isGrounded = false;
        
        currentWeapon = GetComponent<Weapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        moveStick.gameObject.SetActive(false);

        //do not change order of fist instantiation
        LFist = SetUpFist(GetComponentInChildren<LFist>());
        RFist = SetUpFist(GetComponentInChildren<RFist>());

        crown = GetComponentInChildren<Crown>().gameObject;
        crown.SetActive(false);

        mmPlayer = GetComponentInChildren<MiniMapPlayer>();

        baseOfCharacter = GetComponentInChildren<BaseOfCharacter>().gameObject.transform;
        baseOfCharacter.transform.position = new Vector3(baseOfCharacter.position.x, baseOfCharacter.position.y - distanceFromGround, baseOfCharacter.transform.position.z);

        GroundCollider = GetComponentInChildren<GroundColliderBone>().gameObject.GetComponent<SphereCollider>();

        anim = GetComponentInChildren<Animator>();

        audioHandler = GetComponent<AudioHandler>();

        Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorNr);
        int colorID = (int)p.CustomProperties["AssignedColor"];
        myMat = LobbyController.lc.availableMaterials[colorID];

        controllerInitialized = true;
        if (PV.IsMine) MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("ControllerInitialized", true);
        PV.RefreshRpcMonoBehaviourCache();
        PhotonNetwork.UseRpcMonoBehaviourCache = true;

    }

    Fist SetUpFist(Component comp)
    {
        AvatarCharacteristics avatarData = avatarCharacteristics;
        GameObject f = Instantiate(avatarData.FistModel, comp.gameObject.transform.position, Quaternion.Euler(90, 90, 90));
        Dictionary<CosmeticType, CosmeticItem> avatarCosmetics = avatarData.info.currentSet.cosmetics;
        if (avatarData.lFist == null) avatarData.lFist = f;
        else
        {
            avatarData.rFist = f;
            avatarData.AssignFistModels(avatarCosmetics[CosmeticType.Fist]);
        }
        f.transform.parent = GameInfo.GI.FistContainer.transform;
        Fist Fist = f.GetComponent<Fist>();
        Fist.Origin = comp.gameObject.transform;
        avatarData.SetFistMaterial(f, LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]].color);
        Fist.InitializeFist(this);
        
        return Fist;
    }

    #endregion

    #region Update Functions
    protected virtual void Update()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        if (!PV.IsMine) return;
        HandleInputs();
        MouseCombat();
        TrackMouse();
        if (!abilityOffCooldown) HandleCooldownTimer();
    }

    protected bool CheckForTimeStop()
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

    protected void TrackHP()
    {
        Color newCol = Color.Lerp(LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]].color, Color.black, 1 - HP);
        AvatarCharacteristics ColorInfo = avatarCharacteristics;
        ColorInfo.UpdateMaterial(newCol);
        ColorInfo.SetFistMaterial(LFist.gameObject, newCol);
        ColorInfo.SetFistMaterial(RFist.gameObject, newCol);
    }

    protected void HandleAnimationValues()
    {
        //set values for aimx and aimy

        if (currentWeapon != null) hasGun = true;
        else if (currentWeapon == null) hasGun = false;
    }

    public void OnCubeStateChange(bool startingRotation, float newZ)
    {
        SetAllComponents(!startingRotation);
        mmPlayer.OnCubeStateChangeMap(startingRotation, newZ);
    }

    #endregion

    #region Winner Handler
    public void Winning(bool isWinner)
    {
        //equip a crown or something
        crown.SetActive(isWinner);
    }
    #endregion

    #region Mouse Tracking / Combat
    protected void MouseCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == null)
            {
                if (LFist.punching && RFist.punching) return;
                GameInfo.GI.StatChange(actorNr, Stat.punchesThrown);
                PV.RPC("RPC_MeleeAttack", RpcTarget.AllBuffered, AimDirection, actorNr, FistToPunch());
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else if (currentWeapon != null)
            {
                currentWeapon.Attack(AimDirection);
            }
        }
        if (Input.GetMouseButtonDown(1)) SignifyKill();
    }

    public virtual void HandleSpecial()
    {

    }

    int FistToPunch()
    {
        if (!LFist.punching && !RFist.punching)
        {
            System.Random rand = new System.Random();
            double percent = rand.NextDouble();
            if (percent > 0.5)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (RFist.punching) return 1;
            else return 0;
        }
    }

    void DamageReaction(Vector3 _impact)
    {
        impact += _impact;
        receivingImpact = true;
        impactInterp = 0f;
    }

    public void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = transform.position.z;
        AimDirection = (MouseWorldPos - transform.position).normalized;
        AimDirection.z = transform.position.z;

        //visual confirmation that gun is moving
    }

    protected void Punch(int numPunch)
    {
        GameInfo.GI.StatChange(actorNr, Stat.punchesThrown);
        PV.RPC("RPC_MeleeAttack", RpcTarget.AllBuffered, AimDirection, actorNr, numPunch);
    }

    public void EquipWeapon(GameObject weapon)
    {
        currentWeapon = weapon.GetComponent<Weapon>();
        RFist.gameObject.SetActive(false);
        LFist.gameObject.SetActive(false);
    }

    #endregion

    #region Death/ Respawn

    protected void HandleDeaths()
    {
        float vertDistance = transform.position.y - Cube.cb.CurrentFace.face.position.y;
        float horizDistance = Mathf.Abs(transform.position.x - Cube.cb.CurrentFace.face.position.x);
        if (/*horizDistance > boundaryDist ||*/ vertDistance < -boundaryDist)// || HP <= 0f)
        {
            Die();
        }
    }
    
    protected void Die()
    {
        isDead = true;
        GameInfo.GI.StatChange(PhotonNetwork.LocalPlayer.ActorNumber, Stat.deaths);
        //explode with color
        SignifyDeath();
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        int spawnPtlocID = UnityEngine.Random.Range(0, list.Length);
        PV.RPC("LoseWeapon_RPC", RpcTarget.All);
        PV.RPC("DieAndRespawn_RPC", RpcTarget.AllBuffered, spawnPtlocID);
        PhotonNetwork.SendAllOutgoingCommands(); 
    }

    void SignifyDeath()
    {
        // screen rumble 
        CameraFollow camScript = Camera.main.GetComponent<CameraFollow>();
        camScript.TriggerShake(0.7f);
    }

    void SignifyKill()
    {
        //play slash sound
        //display image briefly
        GameManager.Manager.TriggerKillIndicator();
        CameraFollow camScript = Camera.main.GetComponent<CameraFollow>();
        camScript.TriggerShake(0.3f);
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (GameInfo.GI.TimeStopped) StartCoroutine(CubeRotationWait());
        else Respawn();
    }

    protected void Respawn()
    {
        if (PV.IsMine)
        {
            isDead = false;
            rb.velocity = Vector3.zero;
        }
        transform.position = respawnPos;
        mmPlayer.gameObject.SetActive(true);
        avatarCharacteristics.SetMaterial(LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(actorNr).CustomProperties["AssignedColor"]]);
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

    protected void SetAllComponents(bool isActive)
    {
        foreach (Renderer display in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (display.GetType() != typeof(ParticleSystemRenderer)) display.enabled = isActive;
        }
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = isActive;
        }
        LFist.gameObject.SetActive(isActive);
        RFist.gameObject.SetActive(isActive);
    }

    #endregion

    #region Movement Functions
    protected virtual void HandleInputs()
    {
        float inputX = Input.GetAxis("Horizontal");
        bool inputY = Input.GetKeyDown(KeyCode.W);
        bool specialInput = Input.GetKeyDown(KeyCode.R);

        if (specialInput && abilityOffCooldown) SpecialAbility();

        if (inputX > 0) directionModifier = 1;
        else if (inputX < 0) directionModifier = 0;

        if (receivingImpact) FreezePositions(false);
        else if (inputX != 0) FreezePositions(false);
        else if (unfreezeForAbility) FreezePositions(false);
        else if (isGrounded) FreezePositions(true);

        if (inputX > 0) gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
        if (inputX < 0) gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
        if (inputY) TryJump();

        if (!unfreezeForAbility) anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));

        tempVel = new Vector3(inputX * speed, rb.velocity.y, 0f) + impact;

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        if (receivingImpact) ImpactHandler();
    }

    public void AlteredGravity()
    {
        if (rb.velocity.y < 0) rb.velocity += Vector3.up * Physics.gravity.y * 0.5f * Time.deltaTime; ;
    }

    public void Move(Vector3 moveVector)
    {
        rb.velocity = moveVector;
    }

    protected void ImpactHandler()
    {
        impactInterp += Time.deltaTime/5;
        impact = Vector3.Lerp(impact, Vector3.zero, impactInterp);
        if (impact.magnitude < 0.1f)
        {
            receivingImpact = false;
            impact = Vector3.zero;
            impactInterp = 0f;
        }
    }

    public void TryJump()
    {
        if (jumpNum <= 0) return;
        FreezePositions(false);
        PV.RPC("JumpAnimation_RPC", RpcTarget.AllBuffered, actorNr);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void JumpAction()
    {
        anim.SetTrigger("Jump");
        rb.velocity = new Vector3(rb.velocity.x, jumpHeightMultiplier, 0f);
        jumpNum -= 1;
    }

    public void FreezePositions(bool freeze)
    {
        if (freeze) rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;            
        else rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ; 
    }
    #endregion

    #region Collision/ Trigger 
    
    protected void OnCollisionEnter(Collision other)
    {
        if (GroundCheck(other, true)) jumpNum = maxJumps;       
    }

    protected void OnCollisionStay(Collision collision)
    {
        GroundCheck(collision, true);
    }

    protected void OnCollisionExit(Collision collision)
    {
        GroundCheck(collision, false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        GameObject otherGO = other.gameObject;
        if (otherGO.tag == "Projectile")
        {
            Projectile proj = otherGO.GetComponent<Projectile>();
            if (proj.owner == actorNr) return;
            audioHandler.Play("", "Slap");

            if (PV.IsMine)
            {
                Vector3 _impact = proj.impactMultiplier * proj.Velocity.normalized;
                _impact.y = impact.y / 4;
                OnDamgeTaken?.Invoke(proj, this);
                DamageReaction(_impact);
                PhotonNetwork.SendAllOutgoingCommands();
                LoseHealth(proj);
                GameInfo.GI.StatChange(proj.owner, Stat.bulletsLanded);
            }

            Destroy(otherGO);
        }
        else if (other.tag == "Fist")
        {          
            Fist fist = other.GetComponent<Fist>();
            if (fist.owner == actorNr) return;
            audioHandler.Play("", "Slap");
            fist.SetCollider(false);

            if (PV.IsMine)
            {
                Vector3 _impact = fist.impactMultiplier * fist.gameObject.GetComponent<Rigidbody>().velocity;
                _impact.y = impact.y / 4;
                OnDamgeTaken?.Invoke(fist, this);
                impact = fist.gameObject.GetComponent<Rigidbody>().velocity.normalized;
                LoseHealth(fist);
                DamageReaction(_impact);
                PhotonNetwork.SendAllOutgoingCommands();
                GameInfo.GI.StatChange(fist.owner, Stat.punchesLanded);
            }

        }
    }


    bool GroundCheck(Collision collision, bool onGround)
    {       
        if (collision.collider.gameObject.layer == 8) // is the collider we are running into a platform
        {
            if (!onGround)
            {
                isGrounded = onGround;
                return false;
            }
            else
            {
                foreach (ContactPoint cp in collision.contacts)
                {
                    if (cp.thisCollider == GroundCollider)
                    {
                        isGrounded = onGround;
                        return true;
                    }
                    else
                    {
                        isGrounded = false;
                        break;
                    }
                }
                return false;
            }
        }
        else return false;
    }

    #endregion

    #region RPC
    [PunRPC]
    public void GetKill_RPC(int myActor)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != myActor) return;
        SignifyKill();
    }


    [PunRPC]
    public void AnimationSetPosition_RPC(Vector3 networkPos)
    {
        if (!PV.IsMine) transform.position = networkPos;
    }

    public void LoseHealth(DamageDealer damager)//, float lostHP)
    {
        float lostHP = damager.damage;
        PV.RPC("LoseHealth_RPC", RpcTarget.AllBuffered, lostHP);
        if (HP - lostHP <= 0)
        {
            GameInfo.GI.StatChange(damager.owner, Stat.kills);
            PV.RPC("GetKill_RPC", RpcTarget.All, damager.owner);
            PhotonNetwork.SendAllOutgoingCommands();
            Die();
        }
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
        mmPlayer.gameObject.SetActive(false);
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        respawnPos = list[locID].position;
        //transform.position = list[locID].position;
        StartCoroutine(SpawnDelay());
    }

    [PunRPC]
    public void LoseHealth_RPC(float lostHP)
    {
        HP -= lostHP;
    }

    [PunRPC]
    public void LoseWeapon_RPC()
    {
        if (currentWeapon == null) return;
        currentWeapon.Remove();
        currentWeapon = null;
        LFist.gameObject.SetActive(true);
        RFist.gameObject.SetActive(true);
    }

    [PunRPC]
    public void RPC_MeleeAttack(Vector3 aimDir, int actorNumber, int whichFist)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        //PUNCH CODE ON FIST
        if (whichFist == 1) playerInfo.playerAvatar.GetComponent<Controller>().RFist.Punch(aimDir);
        else playerInfo.playerAvatar.GetComponent<Controller>().LFist.Punch(aimDir);
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

    public virtual void SpecialAbility()
    {
        abilityOffCooldown = false;
    }

    public abstract void HandleCooldownTimer();
}

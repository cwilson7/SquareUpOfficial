using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{

    public bool rocketMode = false;
    Vector3 rocketDirection, ogRotation;
    public float rocketVelocity, startingVelocity = 2f, maxVelocity = 75f, rocketAcceleration = 1.05f, cooldownTimer, abilityCooldown = 1f, rotateSpeed = 2f;
    GameObject abilityEmitter;

    override public void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
        ogRotation = avatarCharacteristics.transform.localRotation.eulerAngles;
        abilityEmitter = Instantiate(Resources.Load<GameObject>(avatarCharacteristics.PathOfEffect(EffectType.Ability)), transform.position, Quaternion.identity);
        abilityEmitter.GetComponent<PointyEmission>().InitializePointyEmission(this);
        abilityEmitter.SetActive(false);
    }

    protected override void Die()
    {
        isDead = true;
        rocketMode = false;
        GameInfo.GI.StatChange(PhotonNetwork.LocalPlayer.ActorNumber, Stat.deaths);
        //explode with color
        SignifyDeath();
        Transform[] list = Cube.cb.CurrentFace.spawnPoints;
        int spawnPtlocID = UnityEngine.Random.Range(0, list.Length);
        PV.RPC("LoseWeapon_RPC", RpcTarget.All);
        PV.RPC("DieAndRespawn_RPC", RpcTarget.AllBuffered, spawnPtlocID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();
        HandleAnimationValues();
        AlteredGravity();

        if (!PV.IsMine) return;
        if (rocketMode) RocketMan();
        else Move(tempVel);
        HandleDeaths();
    }

    protected override void Update()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        if (!PV.IsMine) return;
        if (rocketMode && Input.GetKeyDown(KeyCode.R))
        {
            rocketMode = false;
            unfreezeForAbility = false;
            abilityEmitter.SetActive(false);
            StartCoroutine(RotateBack());           
        }
        HandleInputs();
        MouseCombat();
        TrackMouse();
        if (!abilityOffCooldown) HandleCooldownTimer();

    }

    IEnumerator RotateBack()
    {
        float elapsedTime = 0;
        Vector3 startingPos = avatarCharacteristics.transform.up;
        float time = 1f;
        while (elapsedTime < time)
        {
            avatarCharacteristics.transform.up = Vector3.Lerp(startingPos, Vector3.up, elapsedTime/time);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        avatarCharacteristics.transform.localRotation = Quaternion.Euler(ogRotation);
    }

    public override void HandleCooldownTimer()
    {
        if (rocketMode) return;
        else cooldownTimer += Time.deltaTime;
        if (cooldownTimer > abilityCooldown)
        {
            abilityOffCooldown = true;
            cooldownTimer = 0f;
        }
    }

    public override void SpecialAbility()
    {
        base.SpecialAbility();
        anim.SetFloat("Velocity", 0);
        rocketDirection = AimDirection;
        unfreezeForAbility = true;
        rocketVelocity = startingVelocity;
        rocketMode = true;
        abilityEmitter.SetActive(true);
    }

    public void RocketMan()
    {
        var step = rotateSpeed * Time.deltaTime;
        avatarCharacteristics.transform.up = Vector3.Lerp(avatarCharacteristics.transform.up, new Vector3(rocketDirection.x, rocketDirection.y, 0.1f), step);
        
        if (rocketVelocity < maxVelocity) rocketVelocity *= rocketAcceleration;
        rb.velocity = rocketDirection * rocketVelocity;
    }

    protected override void HandleInputs()
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

        if (!rocketMode)
        {
            if (inputX > 0) gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
            if (inputX < 0) gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
            if (inputY) TryJump();
        }

        if (!unfreezeForAbility) anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));

        tempVel = new Vector3(inputX * speed, rb.velocity.y, 0f) + impact;

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        if (receivingImpact) ImpactHandler();      
    }
}
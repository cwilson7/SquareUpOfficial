using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{

    bool rocketMode = false;
    Vector3 rocketDirection, ogRotation;
    float rocketVelocity, startingVelocity = 2f, maxVelocity = 75f, rocketAcceleration = 1.05f, cooldownTimer, abilityCooldown = 1f, rotateSpeed = 2f;

    override public void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
        ogRotation = avatarCharacteristics.transform.localRotation.eulerAngles;
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            rocketMode = false;
            unfreezeForAbility = false;
            avatarCharacteristics.transform.localRotation = Quaternion.Euler(ogRotation);
        }
        HandleInputs();
        MouseCombat();
        TrackMouse();
        if (!abilityOffCooldown) HandleCooldownTimer();

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
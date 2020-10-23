using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{

    bool rocketMode = false;
    Vector3 rocketDirection;
    float rocketVelocity = 2f, ogVelocity, rocketAcceleration = 1.05f, cooldownTimer, abilityCooldown = 1f;

    override public void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
        ogVelocity = rocketVelocity;
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
        rocketDirection = tempVel.normalized;
        rocketVelocity = ogVelocity;
        rocketMode = true;
        // trigger rocket ship mode
    }

    public void RocketMan()
    {
        rocketVelocity *= rocketAcceleration;
        rb.velocity = rocketDirection * rocketVelocity;

        if (Input.GetKeyDown(KeyCode.R))
        {
            rocketMode = false;
        }

    }
}
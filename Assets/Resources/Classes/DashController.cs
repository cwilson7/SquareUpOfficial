using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : Controller
{
    //Special on iphone triggered by shaking phone
    public float dashTime = 0.2f, dashSpeed = 80f, dashTimer = 0f;
    //vector decided by direction player is moving
    Vector2 dashVector;
    bool dashing = false;
    float cooldownTimer = 0f, abilityCooldown = 1f;
    Vector3 dashVelocity;

    void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();
        HandleAnimationValues();
        AlteredGravity();

        if (!PV.IsMine) return;
        if (!dashing) Move(tempVel);
        else HandleDash();
        HandleDeaths();
    }

    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Dash";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {
        //for iphone: dashVector = moveStick.Direction;
        base.SpecialAbility();
        dashVector = new Vector2(AimDirection.x, AimDirection.y).normalized;
        dashing = true;
        unfreezeForAbility = true;
        dashVelocity = dashVector * dashSpeed;
    }

    void HandleDash()
    {
        rb.velocity = dashVelocity;
        dashTimer += Time.deltaTime;
        if (dashTimer > dashTime)
        {
            //end dash
            dashing = false;
            dashTimer = 0f;
            unfreezeForAbility = false;
        }
    }

    public override void HandleCooldownTimer()
    {
        if (dashing) return;
        else cooldownTimer += Time.deltaTime;
        if (cooldownTimer > abilityCooldown)
        {
            abilityOffCooldown = true;
            cooldownTimer = 0f;
        }
    }
}

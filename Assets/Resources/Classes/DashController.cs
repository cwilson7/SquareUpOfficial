using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class DashController : Controller
{
    //Special on iphone triggered by shaking phone
    public float dashTime = 0.2f, dashSpeed = 80f, dashTimer = 0f, effectSpeedThreshold = 20f;
    //vector decided by direction player is moving
    Vector2 dashVector;
    bool dashing;
    float cooldownTimer = 0f, abilityCooldown = 1f;
    Vector3 dashVelocity;
    GameObject specialEffect;

    void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();
        HandleAnimationValues();
        AlteredGravity();
        if (dashing) AbilityEffectHandler();

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
        specialEffect = Resources.Load<GameObject>(avatarCharacteristics.PathOfEffect(EffectType.Ability));
    }

    public override void SpecialAbility()
    {
        //for iphone: dashVector = moveStick.Direction;
        base.SpecialAbility();
        anim.SetFloat("Velocity", 0);
        dashVector = new Vector2(AimDirection.x, AimDirection.y).normalized;
        PV.RPC("DashBool_RPC",RpcTarget.All, true);
        PhotonNetwork.SendAllOutgoingCommands();
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
            PV.RPC("DashBool_RPC", RpcTarget.All, false);
            PhotonNetwork.SendAllOutgoingCommands();
            dashTimer = 0f;
            unfreezeForAbility = false;
        }
    }

    void AbilityEffectHandler()
    {
        if (rb.velocity.magnitude >= effectSpeedThreshold)
        {
            GameObject effect = Instantiate(specialEffect, transform.position, Quaternion.Euler(-rb.velocity.normalized));
            if (effect.GetComponent<DashEmission>() == null) Debug.Log("DASH ABILITY MISSING DashEmission SCRIPT.");
            else effect.GetComponent<DashEmission>().SetColor(myMat);
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

    #region RPCs
    [PunRPC]
    public void DashBool_RPC(bool isDashing)
    {
        dashing = isDashing;
    }
    #endregion
}

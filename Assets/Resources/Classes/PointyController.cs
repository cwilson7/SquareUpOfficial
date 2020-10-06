using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{

    override public void InitializePlayerController()
    {
        base.InitializePlayerController();
        jumpHeightMultiplier *= 1.3f;
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
    }

    void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();
        HandleAnimationValues();
        AlteredGravity();

        if (!PV.IsMine) return;
        Move(tempVel);
        HandleDeaths();
    }

    public override void SpecialAbility()
    {
        base.SpecialAbility();
    }

    public override void HandleCooldownTimer()
    {
        
    }
}
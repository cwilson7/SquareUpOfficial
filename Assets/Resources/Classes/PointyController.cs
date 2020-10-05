using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{
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

    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {
        base.SpecialAbility();
    }

    public override void HandleCooldownTimer()
    {
        
    }
}
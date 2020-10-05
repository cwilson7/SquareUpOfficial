using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyController : Controller
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
        audioKey = "Dash";
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

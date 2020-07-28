using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyController : Controller
{
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Dash";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {

    }
}

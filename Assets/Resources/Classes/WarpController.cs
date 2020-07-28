using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : Controller
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

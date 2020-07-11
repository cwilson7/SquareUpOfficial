using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooseController : Controller
{
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        maxJumps = 4;
        audioKey = "Reginald";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {

    }
}

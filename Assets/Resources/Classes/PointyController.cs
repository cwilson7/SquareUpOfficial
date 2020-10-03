using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyController : Controller
{
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Pointy";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {

    }
}
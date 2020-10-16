using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : Controller
{
    //Special on iphone triggered by shaking phone
    public float warpTime = 0.7f, warpDistance = 10f, warpTimer = 0f;
    //vector decided by direction player is moving
    Vector2 warpVector;
    bool warping = false;
    float cooldownTimer = 0f, abilityCooldown = 1f;

    GameObject warpIndicatorPrefab, currentIndicator;

    private void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (!controllerInitialized) return;
        if (CheckForTimeStop()) return;
        TrackHP();
        HandleAnimationValues();
        if (!warping) AlteredGravity();

        if (!PV.IsMine) return;
        if (!warping) Move(tempVel);
        else HandleWarp();
        HandleDeaths();
    }

    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        audioKey = "Dash";
        audioHandler.InitializeAudio(audioKey);
        warpIndicatorPrefab = Resources.Load<GameObject>(avatarCharacteristics.PathOfEffect(EffectType.Ability));
    }

    public override void SpecialAbility()
    {
        //for iphone: warpVector = moveStick.Direction;
        base.SpecialAbility();
        warpVector = new Vector2(AimDirection.x, AimDirection.y).normalized;
        warping = true;
        unfreezeForAbility = true;
        Warp(warpVector * warpDistance);
    }

    void Warp(Vector3 vector)
    {
        Camera.main.GetComponent<CameraFollow>().smoothDamp += warpTime;
        rb.isKinematic = true;
        SetAllComponents(false);
        transform.position += vector;
        RFist.gameObject.transform.position = RFist.Origin.position;
        LFist.gameObject.transform.position = LFist.Origin.position;
        currentIndicator = Instantiate(warpIndicatorPrefab, transform.position, Quaternion.identity);
        if (currentIndicator.GetComponent<WarpIndicator>() == null) Debug.Log("portal missing WarpIndicator script.");
        else currentIndicator.GetComponent<WarpIndicator>().InitializeIndicator(warpTime, myMat);
    }

    void EndWarp()
    {
        CameraFollow follow = Camera.main.GetComponent<CameraFollow>();
        follow.smoothDamp -= warpTime;
        rb.isKinematic = false;
        SetAllComponents(true);
    }

    void HandleWarp()
    {
        warpTimer += Time.deltaTime;
        if (warpTimer > warpTime)
        {
            //end warp
            warping = false;
            warpTimer = 0f;
            unfreezeForAbility = false;
            EndWarp();
        }
    }

    public override void HandleCooldownTimer()
    {
        if (warping) return;
        else cooldownTimer += Time.deltaTime;
        if (cooldownTimer > abilityCooldown)
        {
            abilityOffCooldown = true;
            cooldownTimer = 0f;
        }
    }
}

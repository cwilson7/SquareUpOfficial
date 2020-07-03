using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyController : Controller
{
    public bool isHookedSide;
    public bool isHookedAbove;
    public Transform sideWallCheck;
    public Transform celingWallCheck;
    public float stickyDetectionRadius;
    public bool hookable;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        sideWallCheck = GetComponentInChildren<SideSticky>().gameObject.transform;
        celingWallCheck = GetComponentInChildren<AboveSticky>().gameObject.transform;
        stickyDetectionRadius = .25f;
        specialCooldown = .1f;
    }

    public override void SpecialAbility()
    {
        hookable = true;
        anim.SetBool("Flip",true);
        StartCoroutine(SpecialTimer(1f));
    }

    public override void Gravity()
    {
        LayerMask ground = LayerMask.GetMask("Platform");
        bool isGrounded = Physics.CheckSphere(baseOfCharacter.position, groundDetectionRadius, ground);
        isHookedSide = Physics.CheckSphere(sideWallCheck.position, stickyDetectionRadius, ground);
        isHookedAbove = Physics.CheckSphere(celingWallCheck.position, stickyDetectionRadius, ground);
        //if (!isHookedAbove || !isHookedSide) hookable = false;
        if ((isGrounded && Velocity.y < 0) || (isHookedAbove && hookable) || (isHookedSide && hookable))
        {
            Velocity.y = 0f;
            jumpNum = maxJumps;
        }
        else Velocity.y += Cube.cb.CurrentFace.GravityMultiplier * gravity * Time.deltaTime;

        if (isHookedSide && hookable) anim.SetBool("SideHooked", true);
        else if (isHookedAbove && hookable) anim.SetBool("AboveHooked", true);
        else
        {
            anim.SetBool("SideHooked", false);
            anim.SetBool("AboveHooked", false);
        }
    }
    public override void Movement()
    {
        HandleInputs(iPhone);
        if (isHookedAbove && hookable) Velocity.x = 0;
        else if(isHookedSide && hookable && directionModifier == 1 && Input.GetKeyDown(KeyCode.A)) Velocity.x = Input.GetAxis("Horizontal");
        else if(isHookedSide && hookable && directionModifier == -1 && Input.GetKeyDown(KeyCode.D)) Velocity.x = Input.GetAxis("Horizontal");
        else Velocity.x = Input.GetAxis("Horizontal");

        Move(Velocity);
    }
    IEnumerator SpecialTimer(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (!isHookedAbove && !isHookedSide)
        {
            anim.SetBool("Flip", false);
            hookable = false;
        }
        else StartCoroutine(SpecialTimer(0.25f));
    }
}

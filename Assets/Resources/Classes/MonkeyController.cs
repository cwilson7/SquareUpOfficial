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
        if (iPhone)
        {
            //Vertical movement
            if (moveStick.Vertical >= 0.8)
            {
                TryJump();
            }

            //Horizontal movement
            if (moveStick.Horizontal >= 0.2)
            {
                Debug.Log("Move");
                Velocity.x = 1;
                //anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (moveStick.Horizontal <= -0.2)
            {
                Velocity.x = -1;
                //anim.SetBool("Run", true);
                gameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                Velocity.x = 0;
                //anim.SetBool("Run", false);
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryJump();
            }
            if (Input.GetKeyDown(KeyCode.S) && isHookedAbove)
            {
                Velocity.y = -1f;
            }
            if ((Input.GetAxis("Horizontal") > 0 || Input.GetKeyDown(KeyCode.D)) && !(directionModifier == 1 && isHookedSide) && !isHookedAbove)
            {
                directionModifier = 1;
                gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
                anim.SetBool("Running", true);
            }
            if ((Input.GetAxis("Horizontal") < 0 || Input.GetKeyDown(KeyCode.A)) && !(directionModifier == -1 && isHookedSide) && !isHookedAbove)
            {
                directionModifier = -1;
                gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
                anim.SetBool("Running", true);
            }
            if (Input.GetAxis("Horizontal") == 0)
            {
                anim.SetBool("Running", false);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TrySpecial();
            }
            if (isHookedAbove && hookable) Velocity.x = 0;
            else if(isHookedSide && hookable && directionModifier == 1 && Input.GetKeyDown(KeyCode.A)) Velocity.x = Input.GetAxis("Horizontal");
            else if(isHookedSide && hookable && directionModifier == -1 && Input.GetKeyDown(KeyCode.D)) Velocity.x = Input.GetAxis("Horizontal");
            else Velocity.x = Input.GetAxis("Horizontal");
        }


        Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
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

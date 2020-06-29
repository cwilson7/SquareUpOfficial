﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class YetiController : Controller
{
    public bool IsInSpecial;
    public float SpecialTime;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        IsInSpecial = false;
        SpecialTime = 5;
    }

    public override void SpecialAbility()
    {
        IsInSpecial = true;
        _Collider.enabled = false;
        anim.SetBool("Special", true);
        speed = 20;
        StartCoroutine(SpecialTimer());
        PV.RPC("Avalanche_RPC", RpcTarget.AllBuffered);
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
            if (!IsInSpecial)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    TryJump();
                }
                if (Input.GetAxis("Horizontal") > 0 || Input.GetKeyDown(KeyCode.D))
                {
                    directionModifier = 1;
                    gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
                    anim.SetBool("Running", true);
                }
                if (Input.GetAxis("Horizontal") < 0 || Input.GetKeyDown(KeyCode.A))
                {
                    directionModifier = -1;
                    gameObject.transform.rotation = Quaternion.Euler(0, -100, 0);
                    anim.SetBool("Running", true);
                }
                if (Input.GetAxis("Horizontal") == 0)
                {
                    anim.SetBool("Running", false);
                }
                Velocity.x = Input.GetAxis("Horizontal");
            }
            else
            {
                directionModifier = 1;
                gameObject.transform.rotation = Quaternion.Euler(0, 100, 0);
                gameObject.transform.rotation = Quaternion.Euler(-90, 0,Mathf.Atan2(AimDirection.y, AimDirection.x) * (180 / Mathf.PI));
                Velocity.x = AimDirection.x;
                Velocity.y = AimDirection.y;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsInSpecial)
                {
                    ExitSpecial();
                }
                else TrySpecial();
            }


        }


        Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public override void Gravity()
    {
        LayerMask ground = LayerMask.GetMask("Platform");
        bool isGrounded = Physics.CheckSphere(baseOfCharacter.position, groundDetectionRadius, ground);
        if (isGrounded && Velocity.y < 0 && !IsInSpecial)
        {
            Velocity.y = 0f;
            jumpNum = maxJumps;
        }
        else if (!IsInSpecial) Velocity.y += Cube.cb.CurrentFace.GravityMultiplier * gravity * Time.deltaTime;
    }

    IEnumerator SpecialTimer()
    {
        yield return new WaitForSeconds(SpecialTime);
        if (IsInSpecial)
        {
            _Collider.enabled = true;
            IsInSpecial = false;
            anim.SetBool("Special", false);
        }
    }

    private void ExitSpecial()
    {
        _Collider.enabled = true;
        IsInSpecial = false;
        anim.SetBool("Special", false);
    }

    [PunRPC]
    public void Avalanche_RPC()
    {
        GameObject avalanche = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "Avalanche"), transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        avalanche.transform.parent = gameObject.transform;
        avalanche.GetComponent<Projectile>().InitializeProjectile(50, 5, Vector3.zero, actorNr);
    }
}

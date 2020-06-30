using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class YetiController : Controller
{
    public float specialForce;
    public bool isInSpecial;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        specialForce = 5f;
    }

    public override void SpecialAbility()
    {
        isInSpecial = true;
        anim.SetBool("Special", true);
        Vector3 dir = AimDirection;
        dir.z = 0;
        PV.RPC("Avalanche_RPC", RpcTarget.AllBuffered,dir);
        if (AimDirection.x > 0) gameObject.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * Mathf.Atan(-AimDirection.y / AimDirection.x), 90, -90);
        else gameObject.transform.localRotation = Quaternion.Euler(180 + Mathf.Rad2Deg * Mathf.Atan(-AimDirection.y / AimDirection.x), 90, -90);
        Velocity.y = AimDirection.y*specialForce;
        Velocity.x = AimDirection.x*specialForce;
        StartCoroutine(SpecialTimer());
        Move(Velocity);
    }

    [PunRPC]
    public void Avalanche_RPC(Vector3 AimDir)
    {
        if (AimDir.x > 0)
        {
            GameObject avalanche = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "YetiBend"), transform.position, Quaternion.Euler(new Vector3(0, 0, 90+Mathf.Rad2Deg * Mathf.Atan(AimDir.y / AimDir.x))));
            avalanche.GetComponent<YetiBend>().aimDirection = AimDir;
        }
        else
        {
            GameObject avalanche = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "YetiBend"), transform.position, Quaternion.Euler(new Vector3(0, 0, 270 + Mathf.Rad2Deg * Mathf.Atan(AimDir.y / AimDir.x))));
            avalanche.GetComponent<YetiBend>().aimDirection = AimDir;
        }
    }

    IEnumerator SpecialTimer()
    {
        yield return new WaitForSeconds(.25f);
        isInSpecial = false;
        anim.SetBool("Special", false);
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
            if (!isInSpecial)
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TrySpecial();
                }
                else Velocity.x = Input.GetAxis("Horizontal");
            }
        }


        Move(Velocity);
        //Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        //cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //lock Z Pos
        transform.position = new Vector3(transform.position.x, transform.position.y, Cube.cb.CurrentFace.spawnPoints[0].position.z);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public override void Gravity()
    {
        if (!isInSpecial)
        {
            LayerMask ground = LayerMask.GetMask("Platform");
            bool isGrounded = Physics.CheckSphere(baseOfCharacter.position, groundDetectionRadius, ground);
            if (isGrounded && Velocity.y < 0)
            {
                Velocity.y = 0f;
                jumpNum = maxJumps;
            }
            else Velocity.y += Cube.cb.CurrentFace.GravityMultiplier * gravity * Time.deltaTime;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;

public abstract class Controller : MonoBehaviour  
{
    public bool iPhone = false;
    
    private PhotonView PV;
    private CharacterController cc;
    [SerializeField] private GameObject baseOfCharacterPrefab; 
    
    public Weapon currentWeapon;
    public Fist Fist;
    
    //Control UI
    protected FloatingJoystick moveStick;

    //Initial Player movement variables
    public int actorNr;
    public float speed, gravity, jumpHeightMultiplier, groundDetectionRadius, distanceFromGround;
    public int maxJumps;
    public Transform baseOfCharacter;
    public float punchPower, punchImpact, punchCooldown;
    
    //Tracked variables
    public Vector3 Velocity, impact;
    public int jumpNum;
    public double HP;
    public Vector3 AimDirection;

    #region SET VALUES
    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerController();
    }

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();

        impact = Vector3.zero;
        AimDirection = Vector2.zero;

        //Default values for all players
        speed = 10f;
        gravity = -9.8f;
        jumpHeightMultiplier = 1f;
        groundDetectionRadius = 0.1f;
        maxJumps = 2;
        distanceFromGround = 0.5f;
        HP = 100;
        punchPower = 10f;
        punchImpact = 1f;
        punchCooldown = 30;
        actorNr = GetComponent<PhotonView>().OwnerActorNr;

        cc = GetComponent<CharacterController>();
        currentWeapon = GetComponent<Weapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        if (iPhone) SwipeDetector.OnSwipe += TouchCombat;
        if (!iPhone) moveStick.gameObject.SetActive(false);

        Fist = GetComponentInChildren<Fist>();
        Fist.InitializeFist();

        baseOfCharacter = GetComponentInChildren<BaseOfCharacter>().gameObject.transform;
        baseOfCharacter.transform.position = new Vector3(baseOfCharacter.position.x, baseOfCharacter.position.y - distanceFromGround, baseOfCharacter.transform.position.z);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //if (GameInfo.GI.TimeStopped) return;
        Gravity();

        if (!PV.IsMine) return;             
        Movement();
        if (!iPhone)
        {
            TrackMouse();
            MouseCombat();
        }
        SpecialAbility();
    }

    public virtual void Gravity()
    {
        LayerMask ground = LayerMask.GetMask("Platform");
        bool isGrounded = Physics.CheckSphere(baseOfCharacter.position, groundDetectionRadius, ground);
        if (isGrounded && Velocity.y < 0)
        {
            Velocity.y = 0f;
            jumpNum = maxJumps;
        }
        else Velocity.y += gravity * Time.deltaTime;
    }

    public virtual void Movement()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        
        if (iPhone)
        {
            //Vertical movement
            if (moveStick.Vertical >= 0.8)
            {
                Jump();
            }

            //Horizontal movement
            if (moveStick.Horizontal >= 0.2)
            {
                Velocity.x = 1;
            }
            else if (moveStick.Horizontal <= -0.2)
            {
                Velocity.x = -1;
            }
            else Velocity.x = 0;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
            Velocity.x = Input.GetAxis("Horizontal");
        }
        
        
        Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    private void MouseCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon == null)
            {
                Fist.Smack(AimDirection);
            }
            else
            {
                currentWeapon.Attack(AimDirection);
            }
        }
    }

    public void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = 0f;
        AimDirection = (MouseWorldPos - transform.position).normalized;
    }

    public void Jump()
    {
        if (jumpNum <= 0) return;
        Velocity.y = Mathf.Sqrt(jumpHeightMultiplier * -1f * gravity);
        jumpNum -= 1;
    }


    #region TOUCH
    public virtual void TouchCombat(SwipeData data)
    {
        Vector3 AttackDirection = data.StartPos - data.EndPos;
        AimDirection = AttackDirection;
        if (currentWeapon == null)
        {
            Fist.Smack(AimDirection);
        }
        else
        {
            currentWeapon.Attack(AimDirection);
        }
    }
    #endregion 
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Projectile proj = other.gameObject.GetComponent<Projectile>();
            if (proj.owner == PV.OwnerActorNr) return;

            if (PV.IsMine)
            {
                LoseHealth(proj.damage);
                GameInfo.GI.StatChange(proj.owner, "bulletsLanded");
            }
            
            impact += proj.impactMultiplier * proj.Velocity.normalized;
            Destroy(other.gameObject);
        }
    }

    #region RPC

    public void LoseHealth(double lostHP)
    {
        PV.RPC("LoseHealth_RPC", RpcTarget.AllBuffered, lostHP);
    }

    [PunRPC]
    public void FireWeapon_RPC(Vector3 Direction, float dmg, float impt, float bltSpd, int ownr, string projName)
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + projName), currentWeapon.FiringPoint.position, Quaternion.identity);
        bullet.GetComponent<Projectile>().InitializeProjectile(dmg, impt, Direction * (float)bltSpd, ownr);
    }

    [PunRPC]
    public void LoseHealth_RPC(double lostHP)
    {
        HP -= lostHP;
    }

    [PunRPC]
    public void LoseWeapon_RPC()
    {
        if (currentWeapon == null) return;
        currentWeapon.Remove();
        currentWeapon = null;
    }

    [PunRPC]
    public void RPC_MeleAttack(Vector3 aim, int actorNumber)
    {
        aim = new Vector3(aim.x, aim.y, 0f);
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actorNumber];
        GameObject fist = playerInfo.playerAvatar.GetComponent<Controller>().Fist.gameObject;
        fist.GetComponent<Fist>().cooldown = Fist.GetComponent<Fist>().timeBtwnPunches;
        fist.GetComponent<SphereCollider>().enabled = true;
        fist.GetComponent<Rigidbody>().AddForce(aim * 1000);
        StartCoroutine(fist.GetComponent<Fist>().FistDrag());
    }
    #endregion

    public abstract void SpecialAbility();
}

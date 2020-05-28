using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System;

public abstract class Controller : MonoBehaviour  
{
    private PhotonView PV;
    private CharacterController cc;
    [SerializeField] private GameObject baseOfCharacterPrefab; 
    
    public EquippedWeapon currentWeapon;
    
    //Control UI
    protected FloatingJoystick moveStick;

    //Initial Player movement variables
    public float speed, gravity, jumpHeightMultiplier, groundDetectionRadius, distanceFromGround;
    public int maxJumps;
    public Transform baseOfCharacter;
    
    //Tracked variables
    protected Vector3 Velocity, impact;
    protected int jumpNum;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerController();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInfo.GI.TimeStopped) return;
        Gravity();
        Movement();
        Combat();
        SpecialAbility();
    }

    public virtual void InitializePlayerController()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine) Destroy(this);

        impact = Vector3.zero;
        speed = 10f;
        gravity = -9.8f;
        jumpHeightMultiplier = 1f;
        groundDetectionRadius = 0.1f;
        maxJumps = 2;
        distanceFromGround = 0.5f;

        cc = GetComponent<CharacterController>();
        currentWeapon = GetComponent<EquippedWeapon>();
        moveStick = JoyStickReference.joyStick.gameObject.GetComponent<FloatingJoystick>();

        baseOfCharacter = GetComponentInChildren<BaseOfCharacter>().gameObject.transform;
        baseOfCharacter.transform.position = new Vector3(baseOfCharacter.position.x, baseOfCharacter.position.y - distanceFromGround, baseOfCharacter.transform.position.z);

        //Default values for all players
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(baseOfCharacter.position, groundDetectionRadius);
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
        //Vertical movement
        if (moveStick.Vertical >= 0.8 && jumpNum > 0)
        {
            Velocity.y = Mathf.Sqrt(jumpHeightMultiplier * -1f * gravity);
            jumpNum -= 1;
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
        Vector3 move = new Vector3(Velocity.x, Velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);

        //Account for impact from being hit by weapon
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    public virtual void Combat()
    {

    }

    public abstract void SpecialAbility();
}

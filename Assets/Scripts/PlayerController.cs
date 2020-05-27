using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private PhotonView PV;

    public CharacterController cc;
    public float speed = 20f;
    public float gravity = -9.8f;
    public float jumpHeight = 1;

    public Transform groundCheck;
    public float groundDistance = 0.6f;
    public LayerMask groundMask;

    public Vector3 velocity;
    Vector2 lMovement;
    bool isGround;
    private int jumpNum;
    private Vector3 impact;

    public GameObject ljoy;
    public GameObject rjoy;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        ljoy = GameObject.Find("LJoyStick");
        impact = Vector2.zero;
        jumpNum = 2;
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGround && velocity.y < 0)
            {
                velocity.y = 0f;
                jumpNum = 2;
            }
            velocity.y += gravity * Time.deltaTime;

            Move();
        }
    }

    private void Move()
    {
        if (ljoy.GetComponent<FloatingJoystick>().Vertical >= 0.8 && jumpNum > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
            jumpNum -= 1;
        }
        velocity.x = ljoy.GetComponent<FloatingJoystick>().Horizontal;
        Vector3 move = new Vector3(velocity.x, velocity.y, 0f);
        cc.Move((move * speed + impact * 10f) * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SexyCubeOfDestinyMouseTrack : MonoBehaviour
{
    public float rotateSpeed;
    public Animator anim;
    public Vector3 AimDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, 0f);
        /*
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = transform.position.z;
        AimDirection = (MouseWorldPos - transform.position).normalized;
        AimDirection.z = transform.position.z;

        anim.SetFloat("AimX", AimDirection.x);
        anim.SetFloat("AimY", AimDirection.y);
        */
    }
}

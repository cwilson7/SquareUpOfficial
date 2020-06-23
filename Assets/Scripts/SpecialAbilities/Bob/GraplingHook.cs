using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GraplingHook : MonoBehaviour
{
    public GameObject hook;
    public GameObject hookHolder;
    public BobController parentController;

    public float playerTravelSpeed;
    public float force;

    public bool fired;

    public float maxDistance;
    private float currrentDistance;

    public Vector3 aimDirection;


    void Start()
    {
        fired = false;
        hookHolder = gameObject.transform.parent.gameObject;
    }


    void Update()
    {
        if (fired == true)
        {
            currrentDistance = Vector3.Distance(transform.position, hook.transform.position);

            if (currrentDistance >= maxDistance)
            {
                Destroy(hook);
                fired = false;
            }
        }
    }

    public void Hooked(Vector3 pos)
    {
        fired = false;
        Destroy(hook);
        parentController.Warp(pos);
    }

    public void Fire(Vector3 Direction)
    {
        fired = true;
        Debug.Log(hookHolder.transform.position);
        hook = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "WarpMissile"), hookHolder.transform.position + new Vector3(0f,2f,0f), Quaternion.identity);
        hook.GetComponent<Rigidbody>().AddForce(Direction * force);
        hook.GetComponent<HookDetector>().parent = this;
    }
}

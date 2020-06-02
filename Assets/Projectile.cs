using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public double damage;
    public Vector3 impact, Velocity;
    public int owner;
    public float lifeTime, maxLifeTime;

    public void InitializeProjectile(double dmg, Vector3 impt, Vector3 vel, int owner)
    {
        this.damage = dmg;
        this.impact = impt;
        this.Velocity = vel;
        this.owner = owner;
        maxLifeTime = 5f;
    }

    void Start()
    {
        lifeTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
        GetComponent<Rigidbody>().velocity = Velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        int otherActorNr = other.gameObject.GetComponentInParent<PhotonView>().OwnerActorNr;
        if (other.gameObject.GetComponentInParent<PhotonView>().OwnerActorNr != owner)
        {
            GameInfo.GI.StatChange(owner, "bulletsLanded");
            other.gameObject.GetComponentInParent<Controller>().LoseHealth(damage);
            other.gameObject.GetComponentInParent<Controller>().impact += impact;
        }
    }
}

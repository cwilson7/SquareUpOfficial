using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageDealer
{
    public float lifeTime, maxLifeTime;

    public void InitializeProjectile(float dmg, float impt, Vector3 vel, int owner)
    {
        maxLifeTime = 5f;
        this.damage = dmg;
        this.impactMultiplier = impt;
        this.Velocity = vel;
        this.owner = owner;
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(owner).CustomProperties["AssignedColor"]];
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (GetComponent<SphereCollider>() == null) return;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}

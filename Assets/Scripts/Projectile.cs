using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageDealer
{
    public float lifeTime, maxLifeTime;

    public virtual void InitializeProjectile(float dmg, float impt, Vector3 vel, int owner)
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
        Vector3 pos = transform.position;
        pos.z = Cube.cb.CurrentFace.spawnPoints[0].position.z;
        transform.position = pos;
    }

    void Start()
    {
        lifeTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody>().velocity = Velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (GetComponent<SphereCollider>() == null) return;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}

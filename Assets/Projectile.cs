using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Vector3 Velocity;
    public float impactMultiplier;
    public int owner;
    public float lifeTime, maxLifeTime;

    public void InitializeProjectile(float dmg, float impt, Vector3 vel, int owner)
    {
        this.damage = dmg;
        this.impactMultiplier = impt;
        this.Velocity = vel;
        this.owner = owner;
        maxLifeTime = 5f;
        GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[LobbyController.lc.selectedMaterialIDs[owner - 1]];
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
}

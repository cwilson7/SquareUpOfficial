using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaynMakr : Projectile
{
    int iteration;
    string thisGOPath;

    public override void InitializeProjectile(float dmg, float impt, Vector3 vel, int owner)
    {
        base.InitializeProjectile(dmg, impt, vel, owner);
        GameObject bullet = Instantiate(Resources.Load<GameObject>(thisGOPath), transform.position, Quaternion.identity);
        RaynMakr makrProj = bullet.GetComponent<RaynMakr>();
        makrProj.InitializeProjectile(damage, impt, vel, owner, iteration);
    }

    public void InitializeProjectile(float dmg, float impt, Vector3 vel, int owner, int newIteration)
    {
        base.InitializeProjectile(dmg, impt, vel, owner);
        this.iteration = newIteration;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

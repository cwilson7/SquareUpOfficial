using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAffected : Projectile
{
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        transform.position += Velocity / Time.deltaTime;
        transform.position += new Vector3(0,-9.8f * lifeTime * lifeTime);
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}

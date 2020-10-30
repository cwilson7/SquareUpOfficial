using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expanding : Projectile
{
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        transform.position += Velocity / Time.deltaTime;
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
        gameObject.transform.localScale = gameObject.transform.localScale + new Vector3(.1f, .1f, .1f);
    }
}
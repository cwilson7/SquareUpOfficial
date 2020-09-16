﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraitFire : Projectile
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
    }
}
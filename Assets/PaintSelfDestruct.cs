﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSelfDestruct : MonoBehaviour
{
    private ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (null != particle)
            if (!particle.IsAlive())
                Destroy(gameObject);
    }
}

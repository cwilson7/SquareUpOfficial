﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPivot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Weapon>().GunPivot = transform;
    }
}

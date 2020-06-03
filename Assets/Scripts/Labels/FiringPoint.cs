using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPoint : MonoBehaviour
{
    void Start()
    {
        GetComponentInParent<Weapon>().FiringPoint = transform;
    }
}

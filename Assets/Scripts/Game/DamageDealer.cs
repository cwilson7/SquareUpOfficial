using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageDealer : MonoBehaviour
{
    public Vector3 Velocity;
    public int owner;
    public float damage, impactMultiplier;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    public Vector3 InitialVelocity;
    ParticleSystem ps;

    public void Initialize(Vector3 Velocity, Material mat)
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = mat.GetColor("_Color");
        main.startSize = Velocity.magnitude/3;
        InitialVelocity = Velocity;

        ps.emissionRate = 4 * Velocity.magnitude * 10;
        ps.Play();
    }
}

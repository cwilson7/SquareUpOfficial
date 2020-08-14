using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    ParticleSystem ps;

    private void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        other.enabled = false;
        ps.transform.rotation = Quaternion.LookRotation(transform.position - other.gameObject.transform.position, Vector3.up);
        ps.Play();
        Debug.Log("Detecting collision");
    }

}

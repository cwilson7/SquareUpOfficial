﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using Photon.Pun;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update
    public bool testing;
    [SerializeField] private List<GameObject> bloodObjs;
    [SerializeField] private LayerMask cubeMask, groundMask;
    public Material mat;
    [SerializeField] private Transform CubeTransform;

    void Start()
    {
        if (testing) return;
        bloodObjs = new List<GameObject>();
        Utils.PopulateList<GameObject>(bloodObjs, "PhotonPrefabs/PaintObjects");
        groundMask = LayerMask.GetMask("Platform");
        cubeMask = LayerMask.GetMask("Cube");
        CubeTransform = Cube.cb.transform;
    }

    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);

        if (other == null) return;
        foreach (ParticleCollisionEvent p in collisionEvents) {
            Debug.Log("Detecting paint collisions on: " + gameObject.name);
            ParticleSystem ps = other.GetComponent<ParticleSystem>();
            var main = ps.main;
            mat = new Material(Shader.Find("Standard"));
            mat.SetColor("_Color", main.startColor.color);

            GameObject bloodObj = bloodObjs[Random.Range(0, bloodObjs.Count)];
            GameObject blood = Instantiate(bloodObj, CubeTransform.position, Quaternion.identity);
            if (!testing) blood.transform.SetParent(Cube.cb.gameObject.GetComponentInChildren<PaintObjects>().gameObject.transform);
            foreach (MeshRenderer b in blood.GetComponentsInChildren<MeshRenderer>()) {
                b.sharedMaterial = mat;
            }

            blood.transform.position = p.intersection;
            blood.transform.localPosition = new Vector3(blood.transform.localPosition.x, blood.transform.localPosition.y, blood.transform.localPosition.z);
            blood.transform.Rotate(Vector3.up, Random.Range(0, 360));
            blood.transform.rotation = Quaternion.FromToRotation(Vector3.up, p.normal);
            float scaleFactor = Random.Range(0.15f, 0.5f);
            blood.transform.localScale.Scale(new Vector3(scaleFactor, 1, scaleFactor));
            bool offEdge = false;
            foreach (Transform t in blood.transform)
            {
                if (!Physics.CheckSphere(t.position, 0.05f, cubeMask) && !Physics.CheckSphere(t.position, 0.05f, groundMask))
                {
                    offEdge = true;
                }
            }
            
            blood.SetActive(true);
            /*
            if (offEdge)
            {
                Destroy(blood);
            } 
            else
            {
                blood.SetActive(true);
            }
            */

        }
    }

    private void OnDrawGizmos()
    {
        if (GetComponent<MeshCollider>() == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh);
    }
}
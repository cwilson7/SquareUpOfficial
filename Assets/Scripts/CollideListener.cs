using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update

    private List<GameObject> bloodObjs;
    [SerializeField] private LayerMask cubeMask, groundMask;
    public Material mat;
    void Start()
    {
        bloodObjs = new List<GameObject>();
        GameInfo.GI.PopulateList<GameObject>(bloodObjs, "PhotonPrefabs/PaintObjects");
        groundMask = LayerMask.GetMask("Platform");
        cubeMask = LayerMask.GetMask("Cube");
    }

    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);

        if (other == null) return;
        foreach (ParticleCollisionEvent p in collisionEvents) {
            ParticleSystem ps = other.GetComponent<ParticleSystem>();
            var main = ps.main;
            mat = new Material(Shader.Find("Standard"));
            mat.SetColor("_Color", main.startColor.color);

            GameObject bloodObj = bloodObjs[Random.Range(0, bloodObjs.Count)];
            GameObject blood = Instantiate(bloodObj, Cube.cb.CurrentFace.face.position, Quaternion.identity);
            blood.transform.SetParent(Cube.cb.gameObject.GetComponentInChildren<PaintObjects>().gameObject.transform);
            foreach (MeshRenderer b in blood.GetComponentsInChildren<MeshRenderer>()) {
                b.sharedMaterial = mat;
            }
            blood.transform.position = p.intersection;
            blood.transform.Rotate(Vector3.up, Random.Range(0, 360));
            blood.transform.rotation = Quaternion.FromToRotation(Vector3.up, p.normal);
            float scaleFactor = Random.Range(0.75f, 1.25f);
            blood.transform.localScale.Scale(new Vector3(scaleFactor, 1, scaleFactor));
            bool offEdge = false;
            foreach (Transform t in blood.transform)
            {
                if (!Physics.CheckSphere(t.position, 0.01f, cubeMask) && !Physics.CheckSphere(t.position, 0.01f, groundMask))
                {
                    offEdge = true;
                }
            }
            
            blood.SetActive(true);
            /*
            if (offEdge)
            {
                Destroy(blood);
                Debug.Log("its' getting destroyed!");
            } 
            else
            {
                Debug.Log("being set active");
                blood.SetActive(true);
            }
            */
        }
    }
}

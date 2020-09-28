using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using Photon.Pun;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update
    public bool testing;
    [SerializeField] public List<GameObject> bloodObjs;
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
            ParticleSystem ps = other.GetComponent<ParticleSystem>();
            var main = ps.main;
            mat = new Material(Shader.Find("Standard"));
            mat.SetColor("_Color", main.startColor.color);

            //vector math
            Vector3 DirectionToCenter = CubeTransform.position - p.intersection;


            //vector math
            GameObject bloodObj = bloodObjs[Random.Range(0, bloodObjs.Count)];
            GameObject blood = Instantiate(bloodObj, CubeTransform.position, Quaternion.identity);
            blood.GetComponent<SpriteRenderer>().color = mat.color;

            blood.transform.position = p.intersection + p.normal * 0.1f;
            if (blood.transform.position == Vector3.zero)
            {
                Destroy(blood);
                return;
            }
            blood.SetActive(true);
            blood.transform.forward = p.normal;
            //blood.GetComponent<PaintInfo>().SetRotation(Quaternion.FromToRotation(Vector3.up, p.normal));
            //blood.transform.Rotate(Vector3.up, Random.Range(0, 360));
            float scaleFactor = Random.Range(0.15f, 0.5f);
            blood.transform.localScale.Scale(new Vector3(scaleFactor, 1, scaleFactor));
            blood.transform.SetParent(Cube.cb.gameObject.GetComponentInChildren<PaintObjects>().gameObject.transform);


        }
    }
}

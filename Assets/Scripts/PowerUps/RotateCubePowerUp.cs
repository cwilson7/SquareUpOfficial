using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCubePowerUp : PowerUp
{
    List<Transform> faces = new List<Transform>();
    float cubeSize, sizeReduction = 120f;
    float rotateSpeed = 60f;
    GameObject cubeHolder;

    void Start()
    {
        cubeHolder = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        
        cubeSize = Cube.cb.cubeSize/ sizeReduction;
        
        //front face faces[0]
        GameObject faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.forward * cubeSize / 2, Quaternion.Euler(new Vector3(0, 180, 0)));
        faces.Add(faceLoc.transform);
        //back face faces[1]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.forward * cubeSize / 2, Quaternion.Euler(new Vector3(0, 180 + 180, 0)));
        faces.Add(faceLoc.transform);
        //right face faces[2]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, 90, 0)));
        faces.Add(faceLoc.transform);
        //left face faces[3]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.right * cubeSize / 2, Quaternion.Euler(new Vector3(0, -90, 0)));
        faces.Add(faceLoc.transform);
        //top face faces[4]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position + transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(180 + 90, 180, 0)));
        faces.Add(faceLoc.transform);
        //bottom face faces[5]
        faceLoc = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/CubeStuff/FaceLocation"), transform.position - transform.up * cubeSize / 2, Quaternion.Euler(new Vector3(180 - 90, 180, 0)));
        faces.Add(faceLoc.transform);

        List<GameObject> levels = Cube.cb.levelModels;

        for (int i = 0; i < faces.Count; i++)
        {
            faces[i].localScale /= sizeReduction;
            GameObject levelModel = Instantiate(levels[i], faces[i].transform);
            levelModel.transform.localScale *= 1.66f;
            Renderer[] renderers = levelModel.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (MonoBehaviour bhvr in renderer.gameObject.GetComponents<MonoBehaviour>())
                {
                    bhvr.enabled = false;
                }
                foreach(Collider cldr in renderer.gameObject.GetComponents<Collider>())
                {
                    cldr.enabled = false;
                }
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
            faces[i].transform.SetParent(cubeHolder.transform);
        }
    }
    
    public override void ItemAbility(int actorNr)
    {
        Cube.cb.StartRotation(actorNr);
    }

    public override void PickUpEffect(Transform transform)
    {

    }

    private void OnDestroy()
    {
        Destroy(cubeHolder);
    }

    // Update is called once per frame
    void Update()
    {
        cubeHolder.transform.Rotate((Vector3.up + Vector3.left) * rotateSpeed * Time.deltaTime);
    }
}

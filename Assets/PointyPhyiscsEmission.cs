using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyPhyiscsEmission : MonoBehaviour
{
    bool dying = false;
    float lifetime = 0f, maxLifetime = 5f, shrinkTime = 1.5f;
    public int[] materialsToChangeIndexes;
    
    public void SetMaterial(Material parentMat)
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] clone = renderer.sharedMaterials; 
        foreach (int index in materialsToChangeIndexes)
        {
            clone[index] = parentMat;
        }
        renderer.sharedMaterials = clone;
    }

    // Start is called before the first frame update
    IEnumerator Dissipate()
    {
        dying = true;
        float elapsedTime = 0;
        Vector3 startingPos = transform.localScale;
        while (elapsedTime < shrinkTime)
        {
            transform.localScale = Vector3.Lerp(startingPos, Vector3.zero, elapsedTime / shrinkTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (dying) return;
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            StartCoroutine(Dissipate());
        }
    }
}

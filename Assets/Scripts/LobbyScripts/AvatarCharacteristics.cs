using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCharacteristics : MonoBehaviour
{
    public void SetMaterial(Material mat)
    {
        foreach (MaterialChange m in gameObject.GetComponentsInChildren<MaterialChange>())
        {
            m.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = new Material[1];
            m.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = mat;
        }
    }

    public Color CurrentColor()
    {
        return gameObject.GetComponentInChildren<MaterialChange>().gameObject.GetComponent<Renderer>().sharedMaterial.color;
    }

    public void UpdateMaterial(Color col)
    {
        foreach (MaterialChange m in gameObject.GetComponentsInChildren<MaterialChange>())
        {
            m.gameObject.GetComponent<Renderer>().material.color = col;
        }
    }
}

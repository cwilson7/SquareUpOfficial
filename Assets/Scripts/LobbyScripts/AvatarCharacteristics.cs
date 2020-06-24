using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCharacteristics : MonoBehaviour
{
    public void SetMaterial(Material mat)
    {
        foreach (MaterialChange m in gameObject.GetComponentsInChildren<MaterialChange>())
        {
            m.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = mat;
        }
    }
}

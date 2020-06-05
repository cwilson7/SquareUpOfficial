using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCharacteristics : MonoBehaviour
{
    public void SetMaterial(Material mat)
    {
        foreach (MeshRenderer m in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            m.sharedMaterial = mat;
        }
    }
}

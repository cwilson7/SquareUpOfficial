using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCharacteristics : MonoBehaviour
{
    public void SetMaterial(Material mat)
    {
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }
}

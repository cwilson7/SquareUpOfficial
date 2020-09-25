using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintInfo : MonoBehaviour
{
    public Transform myTransform;
    public int ID;

    public void SetRotation(Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
    }
}

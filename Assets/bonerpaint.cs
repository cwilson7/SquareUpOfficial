using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bonerpaint : MonoBehaviour
{
    [Range (0f, 360f)]
    public float rotx = 90, roty, rotz;

    private void Start()
    {
        gameObject.transform.rotation = Quaternion.Euler(rotx, roty, rotz);
    }

    public void RotateSprite()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        gameObject.transform.rotation = Quaternion.Euler(angles.x + 90, angles.y, angles.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintInfo : MonoBehaviour
{
    public Transform myTransform;
    public int ID;

    public void SetRotation(Vector3 rotation)
    {
        gameObject.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        Debug.Log("I am located at: " + transform.position + '\n' + "Setting rotation to: " + transform.localRotation.eulerAngles);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionJibberish : MonoBehaviour
{
    [Range(0, 360)]
    [SerializeField] float rotX, rotY, rotZ;

    [SerializeField] GameObject[] vectors;
    [SerializeField] Vector3 toVec;
    [SerializeField] Direction fromVec;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);

        foreach (GameObject vector in vectors)
        {
            Vector3 vec;
            if (fromVec == Direction.up) vec = Vector3.up;
            else if (fromVec == Direction.forward) vec = Vector3.forward;
            else vec = Vector3.right;
            vector.transform.localRotation = Quaternion.FromToRotation(vec, toVec);
        }
    }
}

public enum Direction
{
    up, 
    forward,
    right
}

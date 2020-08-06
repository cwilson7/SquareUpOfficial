using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boner : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float lerpLoc;
    [SerializeField] Material OGMat;

    private void Update()
    {
        GetComponent<Renderer>().material.color = Color.Lerp(OGMat.color, Color.black, lerpLoc);
    }

}

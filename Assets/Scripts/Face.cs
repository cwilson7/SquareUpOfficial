using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public int num;
    public Transform CubeRotation, Location;
    public Level level;

    private void Start()
    {
        Location = this.transform;
    }
}

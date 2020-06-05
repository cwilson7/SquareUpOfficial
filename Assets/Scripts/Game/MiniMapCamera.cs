using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public static MiniMapCamera mmCamera;
    private void Awake()
    {
        mmCamera = this;
    }
    // Start is called before the first frame update
    public void InitializeMiniMapCamera()
    {
        GetComponent<Camera>().orthographicSize = Cube.cb.gameObject.transform.localScale.x/2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

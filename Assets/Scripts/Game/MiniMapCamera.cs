using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapCamera : MonoBehaviour
{
    public static MiniMapCamera mmCamera;
    public RawImage projectedBlank;

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

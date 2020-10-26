using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (GetComponentInChildren<LoadingPanel>() == null) Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

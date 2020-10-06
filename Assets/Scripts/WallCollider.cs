using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    GameObject parentGO;
    
    // Start is called before the first frame update
    public void InitializeCollider(GameObject instantiator)
    {
        parentGO = instantiator;
        gameObject.layer = 17;
    }

    private void Update()
    {
        transform.position = parentGO.transform.position;
    }

}

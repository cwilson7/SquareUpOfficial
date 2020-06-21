using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPendulem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponentInChildren<Light>().enabled)
        {

            var temp = transform.rotation.eulerAngles;
            temp.z = Mathf.PingPong(Time.time/4f, 1f) * 90f + 45f;
            transform.rotation = Quaternion.Euler(temp);
        }
    }
}

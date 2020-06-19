using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingPendulem : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody body;
    public float leftPush;
    public float rightPush;
    public float velThreshold;
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Push();
    }
    public void Push()
    {
        if (transform.rotation.y > -90 && transform.rotation.y < rightPush && (body.angularVelocity.y > 0) && body.angularVelocity.y < velThreshold)
        {
            //body.angularVelocity = new Vector3(0, 0, velThreshold);
            body.AddForce(new Vector3(velThreshold, velThreshold, velThreshold));
            Debug.Log("swing");
        }
        else if (transform.rotation.y < -90 && transform.rotation.y > leftPush && (body.angularVelocity.y < 0) && body.angularVelocity.y > velThreshold*-1)
        {
            body.AddForce(new Vector3(velThreshold, 0, 0) *-1);
        }
    }
}

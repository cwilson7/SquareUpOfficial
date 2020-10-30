using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expanding : Projectile
{
    public float minSize, maxSize;

    bool velocitySet = false;
    Vector3 ogVelocity;


    // Start is called before the first frame update
    private void FixedUpdate()
    {
        if (!velocitySet)
        {
            ogVelocity = Velocity;
            velocitySet = true;
        }
        transform.position += Velocity / Time.deltaTime;
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
        gameObject.transform.localScale = Vector3.Lerp(Vector3.one * minSize, Vector3.one * maxSize, lifeTime / maxLifeTime);
        Velocity = ogVelocity * (1 - Mathf.Log(1 + lifeTime / maxLifeTime));

        //gameObject.transform.localScale = gameObject.transform.localScale + new Vector3(.1f, .1f, .1f);
    }
}
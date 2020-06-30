using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage;
    public float impact;
    public int owner;
    public float maxLife;
    public float life;
    void Start()
    {
        life = 0;
    }
    public void setValues(float dmg, float imp, int own, float lif)
    {
        damage = dmg;
        impact = imp;
        owner = own;
        maxLife = lif;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        life += Time.deltaTime;
        if (life > maxLife)
        {
            Destroy(this.gameObject);
        }
    }
}

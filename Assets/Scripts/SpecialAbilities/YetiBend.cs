using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YetiBend : MonoBehaviour
{
    // Start is called before the first frame update
    public float life;
    public float maxLife;
    public float growthFactor;
    public float moveSpeed;
    public Vector3 aimDirection;
    void Start()
    {
        life = 0f;
        maxLife = .25f;
        growthFactor = 100f;
        moveSpeed = 20;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        life += Time.deltaTime;
        if (life >= maxLife)
        {
            Destroy(this.gameObject);
        }
        Vector3 g = new Vector3(0,growthFactor,0);
        gameObject.transform.localScale += Time.deltaTime * g;
        gameObject.transform.position += aimDirection * moveSpeed * Time.deltaTime;
    }
}

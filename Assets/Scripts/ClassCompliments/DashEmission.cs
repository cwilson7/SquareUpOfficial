using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEmission : MonoBehaviour
{
    public int[] materialIndexesToChange;
    public float duration = 5f, t;
    float endSize = 0f;

    // Update is called once per frame
    void Update()
    {
        InterpSize();
    }

    void InterpSize()
    {
        t += Time.deltaTime / duration;
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.Lerp(scale, endSize * scale, t);
        if (t >= 1) Destroy(this.gameObject);
    }

    public void SetColor(Material mat)
    {
        Renderer renderer = GetComponent<Renderer>();
        for (int i = 0; i < materialIndexesToChange.Length; i++)
        {
            int materialIndex = materialIndexesToChange[i];
            Material[] clone = renderer.sharedMaterials;
            clone[materialIndex] = mat;
            renderer.sharedMaterials = clone;
        }
    }
}

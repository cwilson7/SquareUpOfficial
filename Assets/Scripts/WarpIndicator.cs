using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpIndicator : MonoBehaviour
{
    public int[] materialIndexesToChange;
    private float duration, t = 0;
    [Range(0, 1)]
    public float sizeSwitchPoint;
    public float maxSizeMagnitude;
    Vector3 endVector;
    bool initialized = false, setSize = false;

    public void InitializeIndicator(float _duration, Material _color)
    {
        SetColor(_color);
        duration = _duration * 2;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
        InterpSize();
    }

    void InterpSize()
    {
        t += Time.deltaTime / duration;
        Vector3 scale = transform.localScale;
        if (t <= sizeSwitchPoint && !setSize)
        {
            //first half of animation
            endVector = maxSizeMagnitude * scale;
            setSize = true;
        }
        else if (t >= sizeSwitchPoint)
        {
            //second half
            t = 0f;
            endVector = Vector3.zero;
        }
        transform.localScale = Vector3.Lerp(scale, endVector, t);
        if (transform.localScale.magnitude <= 0.01) Destroy(this.gameObject);
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

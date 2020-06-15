using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTesting : MonoBehaviour
{
    private readonly Color c_color = new Color(0, 0, 0, 0);
    int textureWidth, textureHeight;
    Texture2D m_texture;
    Material m_material;
    bool isEnabled;
    Color color;

    private void Start()
    {
        //`... (correct renderer and material availability check) ...
        m_texture = new Texture2D(textureWidth, textureHeight);
        for (int x = 0; x<textureWidth; ++x)
            for (int y = 0; y<textureHeight; ++y)
                m_texture.SetPixel(x, y, color);
        m_texture.Apply();

        m_material.SetTexture("_DrawingTex", m_texture);
        isEnabled = true;
    }

    private void Update()
    {
        TrackMouse();
    }
    /*
    public void PaintOn(Vector2 textureCoord, Texture2D splashTexture)
    {
        if (isEnabled)
        {
            int x = (int)(textureCoord.x * textureWidth) - (splashTexture.width / 2);
            int y = (int)(textureCoord.y * textureHeight) - (splashTexture.height / 2);
            for (int i = 0; i < splashTexture.width; ++i)
                for (int j = 0; j <   0)
                {
                    Color result = Color.Lerp(existingColor, targetColor, alpha);   // resulting color is an addition of splash texture to the texture based on alpha
                    result.a = existingColor.a + alpha;                             // but resulting alpha is a sum of alphas (adding transparent color should not make base color more transparent)
                    m_texture.SetPixel(newX, newY, result);
                }
        }

        m_texture.Apply();
    }
    */
private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);
        
        if (other == null) return;
        foreach (ParticleCollisionEvent p in collisionEvents)
        {
            
        }
    }

    void TrackMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {

            }
        }
    }

int FindClosestPointIndex(Vector3 point, Vector3[] vectors)
    {
        (int, Vector3) smallestDiff = (-1, new Vector3(1000f, 1000f, 1000f));
        for (int i = 0; i < vectors.Length; i++)//each (Vector3 vector in vectors)
        {
            Vector3 newDiff = Diff(point, vectors[i]);
            if (newDiff.magnitude < smallestDiff.Item2.magnitude) smallestDiff = (i, newDiff);
        }
        return smallestDiff.Item1;
    }

    Vector3 Diff(Vector3 vec1, Vector3 vec2)
    {
        float x = Mathf.Abs(vec1.x - vec2.x);
        float y = Mathf.Abs(vec1.y - vec2.y);
        float z = Mathf.Abs(vec1.z - vec2.z);
        return new Vector3(x, y, z);
    }
}

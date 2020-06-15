using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTesting : MonoBehaviour
{

    private void Start()
    {
    }

    private void Update()
    {
        TrackMouse();
    }

    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);
        /*
        if (other == null) return;
        foreach (ParticleCollisionEvent p in collisionEvents)
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] verticies = mesh.vertices;
            int VertexToChangeIndex = FindClosestPointIndex(p.intersection, verticies);
            colors[VertexToChangeIndex] = Color.red;
            mesh.colors = colors;
        }
        */
    }

    void TrackMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Mesh mesh = GetComponent<MeshFilter>().mesh;
                Vector3[] verticies = mesh.vertices;
                int VertexToChangeIndex = FindClosestPointIndex(hit.point, verticies);
                colors[VertexToChangeIndex] = Color.red;
                mesh.colors = colors;
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

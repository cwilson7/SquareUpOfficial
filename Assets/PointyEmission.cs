using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyEmission : MonoBehaviour
{
    PointyController parentController;
    public string emissionFolderPath;
    GameObject[] physicsEmissions;
    public float emissionPerSec, maxEmissionPerSec, startingEmissionPerSec, minSize, maxSize, sizeInterpSpeed, emissionInstanDistance, physicsEmitTimer = 0f, physicsEmissionMinSz = 0.5f, physicsEmissionMaxSz = 1.5f;

    public void InitializePointyEmission(PointyController _parent)
    {
        parentController = _parent;
        physicsEmissions = Resources.LoadAll<GameObject>(emissionFolderPath);
        transform.localScale = Vector3.one * (maxSize + minSize / 2);
    }

    void InterpSize()
    {
        float t = sizeInterpSpeed * Time.deltaTime;
        if (Random.value > 0.5) transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxSize, t);
        else transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * minSize, t);
    }

    void SpitOutPhysicsObject()
    {
        Vector3 physicsPos = transform.position;
        Vector3 circle = Random.insideUnitSphere;
        physicsPos += new Vector3(circle.x, circle.y, 0f) * emissionInstanDistance;

        System.Random rnd = new System.Random();
        int randIndex = rnd.Next(0, physicsEmissions.Length);
        GameObject physicsEmit = Instantiate(physicsEmissions[randIndex], physicsPos, Quaternion.identity);
        physicsEmit.transform.localScale *= Random.Range(physicsEmissionMinSz, physicsEmissionMaxSz);
    }

    // Update is called once per frame
    void Update()
    {
        if (parentController == null || !parentController.rocketMode) return;

        transform.position = parentController.transform.position;

        InterpSize();

        emissionPerSec = Mathf.Lerp(startingEmissionPerSec, maxEmissionPerSec, parentController.rocketVelocity / parentController.maxVelocity);
        if (physicsEmitTimer > 1 / emissionPerSec)
        {
            physicsEmitTimer = 0;
            SpitOutPhysicsObject();
        }        
        physicsEmitTimer += Time.deltaTime;
    }
}

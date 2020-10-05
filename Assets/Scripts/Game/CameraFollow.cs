using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float distanceFromMap = 40, smoothDamp = 0f;
    private bool rdyToFollow = false;
    private Vector3 offset, velocity = Vector3.zero;
    [SerializeField] private float leeway, maxDistance;

    float rumbleTimer = 0f, shakeMagnitude = 1.2f;
    Vector3 initialPos;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeCameraFollow();
    }

    void InitializeCameraFollow()
    {
        if (GameInfo.GI.scoreTable == null || !GameInfo.GI.scoreTable.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber)) StartCoroutine(InformationDelay());
        else
        {
            Score playerInfo = (Score)GameInfo.GI.scoreTable[PhotonNetwork.LocalPlayer.ActorNumber];
            if (playerInfo.playerAvatar == null) StartCoroutine(InformationDelay());
            else
            {
                maxDistance = Cube.cb.cubeSize / 2 + leeway;
                offset = new Vector3(0f, 0f, -distanceFromMap);
                player = playerInfo.playerAvatar;
                rdyToFollow = true;
            }
        }
    }

    IEnumerator InformationDelay()
    {
        yield return new WaitForSeconds(0.1f);
        InitializeCameraFollow();
    }

    public void TriggerShake(float shakeTime)
    {
        rumbleTimer += shakeTime;
        initialPos = transform.localPosition;
    }

    void Follow()
    {
        if (Cube.cb == null) return;

        //some buffer for the y desired pos

        Vector3 desiredPos = player.transform.position + offset;
        Vector3 center = Cube.cb.CurrentFace.face.position;
        float horizDist = Mathf.Abs(desiredPos.x - center.x);
        float vertDist = Mathf.Abs(desiredPos.y - center.y);
        if (horizDist > maxDistance) {
            if (desiredPos.x < 0) desiredPos.x = center.x - maxDistance;
            else desiredPos.x = center.x + maxDistance;
        }
        if (vertDist > maxDistance)
        {
            if (desiredPos.y < 0) desiredPos.y = center.y - maxDistance;
            else desiredPos.y = center.y + maxDistance;
        }
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothDamp);
        transform.position = smoothedPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Cube.cb == null) return;
        if (rumbleTimer > 0)
        {
            Vector2 randCircle = Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = new Vector3(initialPos.x + randCircle.x, initialPos.y + randCircle.y, initialPos.z);
            rumbleTimer -= Time.deltaTime;
        }
        else if (rdyToFollow && !Cube.cb.inRotation && player.GetComponent<Controller>().isDead == false) Follow();
        else if (rdyToFollow && Cube.cb != null && Cube.cb.inRotation)
        {
            Vector3 pos = Cube.cb.transform.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z - Cube.cb.cubeSize - distanceFromMap * 3);
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float distanceFromMap = 20, smoothDamp;
    private bool rdyToFollow = false;
    private Vector3 offset, velocity = Vector3.zero;
    [SerializeField] private float leeway, maxDistance;
    
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

    void Follow()
    {
        if (Cube.cb == null) return;
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
        if (rdyToFollow && !Cube.cb.inRotation && player.GetComponent<Controller>().isDead == false) Follow();
    }
}

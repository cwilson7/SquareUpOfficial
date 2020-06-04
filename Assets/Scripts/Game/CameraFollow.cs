using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float distanceFromMap = 13, smoothDamp;
    private bool rdyToFollow = false;
    private Vector3 offset, velocity = Vector3.zero;
    
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
        Vector3 desiredPos = player.transform.position + offset;
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothDamp);
        transform.position = smoothedPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (rdyToFollow) Follow();
    }
}

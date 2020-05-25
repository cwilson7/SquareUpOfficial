using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CarouselBehaviour : MonoBehaviour
{
    public List<Transform> playerDisplayLocations;

    public Transform displayLocationPrefab;
    
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayerDisplay()
    {
        List<GameObject> avatarList = LobbyController.lc.charAvatars;
        CarouselController controller = CarouselController.cc;
        playerDisplayLocations = new List<Transform>();

        Debug.Log("avatar list size is " + avatarList.Count);

        for (int index = 0; index < avatarList.Count; index++)
        {
            Transform location = Instantiate(displayLocationPrefab, new Vector3(controller.carouselRadius * Mathf.Cos(2*Mathf.PI / avatarList.Count * index), controller.gameObject.transform.position.y, controller.carouselRadius * Mathf.Sin(2*Mathf.PI / avatarList.Count * index) + controller.distanceFromCamera), Quaternion.identity);
            playerDisplayLocations.Add(location);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CarouselBehaviour : MonoBehaviour
{
    public List<Transform> playerDisplayLocations;
    public float distanceMultiplier;

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

        for (int index = 0; index < avatarList.Count; index++)
        {
            float distanceX = controller.carouselRadius / 2 * Mathf.Cos(2 * Mathf.PI / avatarList.Count * (index+3)) * distanceMultiplier;
            float distanceZ = controller.carouselRadius / 2 * Mathf.Sin(2 * Mathf.PI / avatarList.Count * (index+3)) * distanceMultiplier;
            
            Transform location = Instantiate(displayLocationPrefab, new Vector3(distanceX, controller.gameObject.transform.position.y, distanceZ + controller.distanceFromCamera), Quaternion.identity);
            location.SetParent(transform);
            playerDisplayLocations.Add(location);
        }
    }
}

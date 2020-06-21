﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            float angle = -2 * Mathf.PI / (avatarList.Count) * index;
            float distanceZ = -controller.carouselRadius / 2 * Mathf.Cos(angle) * distanceMultiplier;
            float distanceX = -controller.carouselRadius / 2 * Mathf.Sin(angle) * distanceMultiplier;
            
            Transform location = Instantiate(displayLocationPrefab, new Vector3(distanceX, controller.gameObject.transform.position.y -1.5f, distanceZ + controller.distanceFromCamera), Quaternion.Euler(0f, -Mathf.Rad2Deg* (Mathf.PI - angle), 0f)); 
            location.SetParent(transform);
            playerDisplayLocations.Add(location);
        }
    }
}

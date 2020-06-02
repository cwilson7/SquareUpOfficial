﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharSelectPanelController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject charPanel, playerListPanel;
    [SerializeField] private float offsetFromLeftEdge, percentThreshold, easing;
    private List<CharPage> characterPages;
    private Vector3 panelLocation;
    private int panelCounter;
    public Hashtable displayedCharacters;
    public Camera charDisplayCamera;
    public Material defaultMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        SetTransformInfo();
        CarouselController.cc.carousel.GetComponent<CarouselBehaviour>().InitializePlayerDisplay();
        for (int i = 0; i < LobbyController.lc.charAvatars.Count; i++)
        {
            GenerateCharacterPanel(i);
        }
    }

    private void GenerateCharacterPanel(int charID)
    {
        GameObject newPanel = Instantiate(charPanel, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2 ((charID + 0.5f) * charPanel.GetComponent<RectTransform>().rect.width, newPanel.GetComponent<RectTransform>().anchoredPosition.y);
        newPanel.GetComponent<CharPage>().ShowDetails(charID);
        characterPages.Add(newPanel.GetComponent<CharPage>());

        GameObject myChar = (GameObject)displayedCharacters[charID];
    }

    private void SetTransformInfo()
    {
        // Sets anchors to middle/ left of canvas
        charPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        charPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);

        panelLocation = transform.position;
        panelCounter = 1;

        displayedCharacters = new Hashtable();

        characterPages = new List<CharPage>();
    }

    public void UpdateCurrentDisplayedCharacter()
    {
        GameObject charToDisplay = null;
        foreach (GameObject character in displayedCharacters.Values)
        {
            if((int) MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"] == -1) character.GetComponent<AvatarCharacteristics>().SetMaterial(defaultMaterial);
            else character.GetComponent<AvatarCharacteristics>().SetMaterial(LobbyController.lc.availableMaterials[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"]]);
        }
        if ((bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"])
        {
            Material assignedMat = LobbyController.lc.availableMaterials[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"]];
            charToDisplay = (GameObject)displayedCharacters[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["SelectedCharacter"]];
            charToDisplay.GetComponent<AvatarCharacteristics>().SetMaterial(assignedMat);
        }
        else if (displayedCharacters.ContainsKey(panelCounter-1))
        {
            charToDisplay = (GameObject)displayedCharacters[panelCounter-1];           
        }
        if(charToDisplay != null) charToDisplay.SetActive(true);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!(bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"])
        {
            float difference = data.pressPosition.x - data.position.x;
            transform.position = panelLocation - new Vector3(difference, 0, 0);
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!(bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"])
        {
            float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newLocation = panelLocation;
                if (percentage > 0 && (panelCounter < LobbyController.lc.charAvatars.Count))
                {
                    newLocation += new Vector3(-Screen.width, 0, 0);
                    panelCounter += 1;
                }
                else if (percentage < 0 && (panelCounter > 0))
                {
                    newLocation += new Vector3(Screen.width, 0, 0);
                    panelCounter -= 1;
                }

                SexyTransition(transform.position, newLocation, easing, easing);
                panelLocation = newLocation;
            }
            else
            {
                SexyTransition(transform.position, panelLocation, easing, easing);
            }
        }
    }

    private void SexyTransition(Vector3 startpos, Vector3 endpos, float panelSeconds, float carouselSeconds)
    {
        if(!(bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"]) StartCoroutine(SexyTransitionCarousel(carouselSeconds));
        StartCoroutine(SexyTransitionPanels(startpos, endpos, panelSeconds));
    }

    IEnumerator SexyTransitionPanels(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f; 
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }
    IEnumerator SexyTransitionCarousel(float seconds)
    {
        float t = 0f;
        Quaternion rotation = Quaternion.Euler(0, Mathf.Rad2Deg * (panelCounter - 1) * 2 * Mathf.PI / (LobbyController.lc.charAvatars.Count), 0);
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;

            if (panelCounter >= 1)
            {
                CarouselController.cc.carousel.transform.rotation = Quaternion.Slerp(CarouselController.cc.carousel.transform.rotation, rotation, Mathf.SmoothStep(0f, 1f, t));
            }
            yield return null;
        }
    }

    public void SendToPlayerList()
    {
        foreach (CharPage page in characterPages)
        {
            page.SetPanelActive(false);
        }
        SexyTransition(transform.position, panelLocation = playerListPanel.transform.position + new Vector3((1 + panelCounter)*Screen.width, 0 ,0), easing, easing);
        panelCounter = 0;
    }

    public void SendToFirstCharacterPanel()
    {
        foreach (CharPage page in characterPages)
        {
            page.SetPanelActive(true);
        }
        panelCounter = 1;
        SexyTransition(transform.position, panelLocation += new Vector3(-Screen.width, 0, 0), easing, easing);
    }

}
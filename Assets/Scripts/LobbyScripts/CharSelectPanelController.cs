using Photon.Pun;
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
    private Vector3 panelLocation;
    private int panelCounter;
    public Hashtable displayedCharacters;
    public Camera charDisplayCamera;
    public Material defaultMaterial;
    public bool charSelected;
    
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

        GameObject myChar = (GameObject)displayedCharacters[charID];
    }

    private void SetTransformInfo()
    {
        // Sets anchors to middle/ left of canvas
        charPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        charPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);

        charSelected = false;

        panelLocation = transform.position;
        panelCounter = 1;

        displayedCharacters = new Hashtable();
    }

    public void UpdateCurrentDisplayedCharacter()
    {
        GameObject charToDisplay = null;
        foreach (GameObject character in displayedCharacters.Values)
        {
            character.GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
        }
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerReady"])
        {
            Material assignedMat = LobbyController.lc.availableMaterials[(int)PhotonNetwork.LocalPlayer.CustomProperties["AssignedColor"]];
            charToDisplay = (GameObject)displayedCharacters[(int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedCharacter"]];
            charToDisplay.GetComponent<MeshRenderer>().sharedMaterial = assignedMat;
        }
        else if (displayedCharacters.ContainsKey(panelCounter-1))
        {
            charToDisplay = (GameObject)displayedCharacters[panelCounter-1];           
        }
        if(charToDisplay != null) charToDisplay.SetActive(true);
    }

    /* color stuff
    private Color CharacterColor(GameObject character)
    {
        Color color = character.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_BaseColor");
        return color;
    }

    private void SetCharacterColor(GameObject character, Color colorToBecome)
    {
        character.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", colorToBecome);
    }
    */

    public void OnDrag(PointerEventData data)
    {
        if (!charSelected)
        {
            float difference = data.pressPosition.x - data.position.x;
            transform.position = panelLocation - new Vector3(difference, 0, 0);
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!charSelected)
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
        if(!charSelected) StartCoroutine(SexyTransitionCarousel(carouselSeconds));
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
        SexyTransition(transform.position, panelLocation = playerListPanel.transform.position + new Vector3((1 + panelCounter)*Screen.width, 0 ,0), easing*4, easing*4);
        panelCounter = 0;
    }

}

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

        panelLocation = transform.position;
        panelCounter = 1;

        displayedCharacters = new Hashtable();
    }

    public void UpdateCurrentDisplayedCharacter()
    {
        GameObject charToDisplay = null;
        foreach (GameObject character in displayedCharacters.Values)
        {
            if(character.activeSelf)
            {
                character.SetActive(false);
            }
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
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);

        Quaternion targetAngle = Quaternion.Euler(0, Mathf.Rad2Deg * ((panelCounter - 1) * 2 * Mathf.PI / LobbyController.lc.charAvatars.Count + ConvertDistanceToRadians(difference)), 0);
        CarouselController.cc.carousel.transform.rotation = Quaternion.Slerp(CarouselController.cc.carousel.transform.rotation, targetAngle, Time.deltaTime);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if(Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if(percentage > 0 && (panelCounter < LobbyController.lc.charAvatars.Count))
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                panelCounter += 1;
            } else if (percentage < 0 && (panelCounter > 0))
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                panelCounter -= 1;
            }
            StartCoroutine(SexyTransition(transform.position, newLocation, easing));
            panelLocation = newLocation;
        } else
        {
            StartCoroutine(SexyTransition(transform.position, panelLocation, easing));
        }
    }

    IEnumerator SexyTransition(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f; 
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;

            if (panelCounter >= 1)
            {
                Quaternion targetAngle = Quaternion.Euler(0, Mathf.Rad2Deg * (panelCounter - 1) * 2 * Mathf.PI / LobbyController.lc.charAvatars.Count, 0);
                CarouselController.cc.carousel.transform.rotation = Quaternion.Slerp(CarouselController.cc.carousel.transform.rotation, targetAngle, Mathf.SmoothStep(0f, 1f, t));
            }

            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }

    private float ConvertDistanceToRadians(float distance)
    {
        float maxDistance = Screen.width/2;
        float radians = distance / maxDistance * 2 * Mathf.PI / LobbyController.lc.charAvatars.Count;
        return radians;
    }

    public void SendToPlayerList()
    {
        StartCoroutine(SexyTransition(transform.position, panelLocation = playerListPanel.transform.position + new Vector3((1+panelCounter)*Screen.width, 0 ,0), easing*4));
        panelCounter = 0;
    }
}

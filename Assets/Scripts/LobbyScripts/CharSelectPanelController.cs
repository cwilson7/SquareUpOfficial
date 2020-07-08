using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharSelectPanelController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //some list of panels which is characterPages

    [SerializeField] private GameObject charPanel, playerListPanel;
    [SerializeField] private float offsetFromLeftEdge, percentThreshold, easing;
    [SerializeField] private List<CharPage> characterPages;
    private Vector3 panelLocation;
    private int panelCounter;
    public Hashtable displayedCharacters;
    public Camera charDisplayCamera;
    public Material defaultMaterial;
    private int swipeCounter = 0;

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

    public bool CheckForDuplicateMaterials()
    {
        List<int> colors = new List<int>();
        int dups;
        foreach (Player entry in PhotonNetwork.CurrentRoom.Players.Values)
        {
            int color = (int)entry.CustomProperties["AssignedColor"];
            if (color != -1) colors.Add(color);
        }
        if (colors.Count() != colors.Distinct().Count())
        {
            Debug.Log("I found duplicate colors");
            List<int> duplicates = colors.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            Debug.Log("The duplicate color IDs are {" + duplicates.ToArray() + "}");
            dups = duplicates.Count();
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (duplicates.Contains((int)player.CustomProperties["AssignedColor"]))
                {
                    Debug.Log("player number " + player.ActorNumber + " has a duplicate color");
                    ResetColorInfo(player);
                    dups -= 1;
                    if (dups == 0) break;
                }
            }
            return true;
        }
        else return false;
    }

    private void ResetColorInfo(Player player)
    {
        int newID = GenerateRandomColorID();
        Debug.Log("Setting player " + player.ActorNumber + " color to " + newID);
        /*
        ExitGames.Client.Photon.Hashtable customProperties = player.CustomProperties;
        customProperties["AssignedColor"] = newID;
        player.SetCustomProperties(customProperties);
        */
        LobbyController.lc.photonView.RPC("ResetColorInfo_RPC", RpcTarget.AllBuffered, player.ActorNumber, newID);
    }

    private int GenerateRandomColorID()
    {
        int maxColors = Mathf.Min(MultiplayerSettings.multiplayerSettings.maxPlayers, LobbyController.lc.availableMaterials.Count);
        int color = UnityEngine.Random.Range(0, maxColors);
        if (LobbyController.lc.selectedMaterialIDs.Contains(color))
        {
            return GenerateRandomColorID();
        }
        else return color;
    }

    private void GenerateCharacterPanel(int charID)
    {
        GameObject newPanel = Instantiate(charPanel, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        if (charID < LobbyController.lc.charAvatars.Count - 1)
        {
            newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2((charID + 0.5f) * charPanel.GetComponent<RectTransform>().rect.width, newPanel.GetComponent<RectTransform>().anchoredPosition.y);
            characterPages.Add(newPanel.GetComponent<CharPage>());
        }
        else
        {
            newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(characterPages[0].gameObject.GetComponent<RectTransform>().anchoredPosition.x - charPanel.GetComponent<RectTransform>().rect.width, newPanel.GetComponent<RectTransform>().anchoredPosition.y);
            characterPages.Insert(0, newPanel.GetComponent<CharPage>());
        }
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

        characterPages = new List<CharPage>();
    }

    public void UpdateCurrentDisplayedCharacter()
    {
        GameObject charToDisplay = null;
        foreach (GameObject character in displayedCharacters.Values)
        {
            if ((int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"] == -1) character.GetComponent<AvatarCharacteristics>().SetMaterial(defaultMaterial);
            else character.GetComponent<AvatarCharacteristics>().SetMaterial(LobbyController.lc.availableMaterials[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"]]);
        }
        if ((bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"])
        {
            Material assignedMat = LobbyController.lc.availableMaterials[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["AssignedColor"]];
            charToDisplay = (GameObject)displayedCharacters[(int)MultiplayerSettings.multiplayerSettings.localPlayerValues["SelectedCharacter"]];
            charToDisplay.GetComponent<AvatarCharacteristics>().SetMaterial(assignedMat);
        }
        else if (displayedCharacters.ContainsKey(panelCounter - 1))
        {
            charToDisplay = (GameObject)displayedCharacters[panelCounter - 1];
        }
        if (charToDisplay != null) charToDisplay.SetActive(true);
    }

    public void OnDrag(PointerEventData data)
    {
        if (!(bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"])
        {
            float difference = data.pressPosition.x - data.position.x;
            transform.position = panelLocation - new Vector3(difference, 0, 0);
        }
    }

    void ShiftPanels(float percentage)
    {
        if (percentage > 0)
        {
            Vector2 lastPageLoc = characterPages[characterPages.Count - 1].gameObject.GetComponent<RectTransform>().anchoredPosition;
            CharPage tmp = characterPages[0];
            characterPages.Add(tmp);
            characterPages.RemoveAt(0);
            RectTransform trans = tmp.gameObject.GetComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(lastPageLoc.x + trans.rect.width, trans.anchoredPosition.y);
        }
        if (percentage < 0)
        {
            Vector2 firstPageLoc = characterPages[0].gameObject.GetComponent<RectTransform>().anchoredPosition;
            CharPage tmp = characterPages[characterPages.Count - 1];
            characterPages.Insert(0, tmp);
            characterPages.RemoveAt(characterPages.Count - 1);
            RectTransform trans = tmp.gameObject.GetComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(firstPageLoc.x - trans.rect.width, trans.anchoredPosition.y);
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
                ShiftPanels(percentage);
                if (percentage > 0)
                {
                    swipeCounter += 1;
                    newLocation += new Vector3(-Screen.width, 0, 0);
                    if (panelCounter < LobbyController.lc.charAvatars.Count) panelCounter += 1;
                    else panelCounter = 1;
                }
                else if (percentage < 0)
                {
                    swipeCounter -= 1;
                    newLocation += new Vector3(Screen.width, 0, 0);
                    if (panelCounter > 1) panelCounter -= 1;
                    else panelCounter = LobbyController.lc.charAvatars.Count;
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
        if (!(bool)MultiplayerSettings.multiplayerSettings.localPlayerValues["PlayerReady"]) StartCoroutine(SexyTransitionCarousel(carouselSeconds));
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
        playerListPanel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        foreach (CharPage page in characterPages)
        {
            page.SetPanelActive(false);
        }
        SexyTransition(transform.position, panelLocation += new Vector3((swipeCounter) * Screen.width, 0, 0), easing, easing);
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
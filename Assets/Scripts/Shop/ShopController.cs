using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour, IDragHandler, IEndDragHandler
{ 
    public static ShopController Instance;
    
    [SerializeField] private GameObject ShopPanelPrefab, charPosPrefab;

    public GameObject mainPanel, displayHolder;

    public List<GameObject> characterPanels;

    public Vector3 charPosInCam = new Vector3(0.94f, -0.46f, 3.2f);
    public float scaleOfChars = 0.4f, percentToSwipe = 0.5f, easing = 0.5f, charSeperation = 15f;

    Vector3 panelLocation, currDisplayLoc;

    public int panelIndex = 0;

    void Start()
    {
        Instance = this;
        panelLocation = transform.position;
        List<GameObject> charAvatars = new List<GameObject>();
        characterPanels = new List<GameObject>();
        Utils.PopulateList<GameObject>(charAvatars, "PhotonPrefabs/CharacterAvatars");
        for (int j = 0; j < charAvatars.Count; j++)
        {
            characterPanels.Add(GenerateShopPanel(charAvatars[j], j));
        }
        mainPanel.GetComponent<MainPanel>().InitializeCharacterButtons();
    }

    GameObject GenerateShopPanel(GameObject _character, int charNum)
    {
        GameObject panel = Instantiate(ShopPanelPrefab, GameObject.Find("Canvas").transform);
        panel.transform.SetParent(this.transform);
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x + Screen.width * charNum, panel.transform.localPosition.y, panel.transform.localPosition.z);
        GameObject charPos = Instantiate(charPosPrefab, Camera.main.transform);
        charPos.transform.localPosition = new Vector3((charNum*charSeperation + 1) * charPosInCam.x, charPosInCam.y, charPosInCam.z);
        charPos.transform.localScale = Vector3.one * scaleOfChars;
        charPos.transform.rotation = Quaternion.Euler(0, 180, 0);
        charPos.transform.SetParent(displayHolder.transform);
        ShopPanel shopPanel = panel.GetComponent<ShopPanel>();
        shopPanel.charLocation = charPos.transform;
        if (charNum == 0) currDisplayLoc = displayHolder.transform.localPosition;
        shopPanel.Character = _character;
        shopPanel.Setup();
        return panel;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScene");        
    }


    void IDragHandler.OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
        float scaledThing = -Scale(difference, -Screen.width, Screen.width, currDisplayLoc.x - charSeperation, currDisplayLoc.x + charSeperation);
        float displayX = scaledThing;
        Debug.Log(scaledThing);
        displayHolder.transform.localPosition = new Vector3(displayX, 0, 0);
    }

    private float Scale(float value, float min, float max, float minScale, float maxScale)
    {
        float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
        return scaled;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentToSwipe)
        {
            Vector3 newLocation = panelLocation;
            Vector3 newCharLoc = currDisplayLoc;
            if (percentage > 0 && panelIndex < characterPanels.Count - 1)
            {
                newLocation += new Vector3(-Screen.width, 0, 0);
                panelIndex++;
                newCharLoc = new Vector3(charSeperation * charPosInCam.x * panelIndex, 0, 0);
            }
            else if (percentage < 0 && panelIndex > 0)
            {
                newLocation += new Vector3(Screen.width, 0, 0);
                panelIndex--;
                newCharLoc = new Vector3(charSeperation * charPosInCam.x * panelIndex, 0, 0);
            }

            Transition(transform.position, newLocation, displayHolder.transform.localPosition, easing);
            panelLocation = newLocation;
            currDisplayLoc = newCharLoc;
        }
        else
        {
            Transition(transform.position, panelLocation, displayHolder.transform.localPosition, easing);
        }
    }

    private void Transition(Vector3 startpos, Vector3 endpos, Vector3 startDisp, float seconds)
    {
        StartCoroutine(ScreenTransition(startpos, endpos, seconds));
        StartCoroutine(CharacterTransition(startDisp, seconds * 0.75f));
    }

    IEnumerator ScreenTransition(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;           
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));                       
            yield return null;
        }
    }

    IEnumerator CharacterTransition(Vector3 startDisp, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            displayHolder.transform.localPosition = Vector3.Lerp(startDisp, new Vector3(-charPosInCam.x * panelIndex * charSeperation, 0, 0), Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}

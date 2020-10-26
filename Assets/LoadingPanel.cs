using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    Material[] colorsToInterp;
    Image background, loadingBuddy;
    RectTransform rectTransform;
    string loadingBuddyPrefabPath = "PhotonPrefabs/GameUI/LoadingPanelBuddy";
    int nextColIndex = 1;
    float colorTimer = 0f, dotTimer = 0f;

    public TMP_Text loadingTxt;
    public string ogText;
    public int maxDots, dots = 0;
    public float dotsPerSec, colorsPerSec, panelScrollTime;

    public bool isDisplayed = true;

    // Start is called before the first frame update
    void Awake()
    {
        //if (SceneManager.GetActiveScene().buildIndex != MultiplayerSettings.multiplayerSettings.intermediateScene || SceneManager.GetActiveScene().buildIndex != MultiplayerSettings.multiplayerSettings.multiplayerScene) Destroy(this.gameObject);
        background = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        GameObject _loadingObject = Instantiate(Resources.Load<GameObject>(loadingBuddyPrefabPath), GetComponentInParent<Canvas>().transform);
        _loadingObject.transform.SetSiblingIndex(transform.GetSiblingIndex());
        loadingBuddy = _loadingObject.GetComponent<Image>();
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(loadingBuddy.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (colorsToInterp == null) colorsToInterp = LobbyController.lc.availableMaterials.ToArray();
        if (Input.GetKeyDown(KeyCode.Space)) DisplayLoadingPanel(!isDisplayed);

        InterpColors(loadingTxt);
        InterpColors(background);
        TextInterp();
    }

    public void DisplayLoadingPanel(bool display)
    {
        StartCoroutine(ScrollInDisplay(display));
        isDisplayed = display;
    }

    IEnumerator ScrollInDisplay(bool toScreen)
    {
        float elapsedTime = 0, time = panelScrollTime, startingAlpha = loadingBuddy.color.a, endingAlpha;
        Vector3 startingPos = rectTransform.anchoredPosition, endingPos;
        Color loadingBuddyColor  = loadingBuddy.color;
        if (toScreen)
        {
            endingPos = Vector2.zero;
            endingAlpha = 1f;
        }
        else
        {
            endingPos = new Vector2(0f, Screen.height);
            endingAlpha = 0f;
        }
        while (elapsedTime < time)
        {
            float step = elapsedTime / time;
            rectTransform.anchoredPosition = Vector2.Lerp(startingPos, endingPos, step);
            float alpha = Mathf.Lerp(startingAlpha, endingAlpha, step);
            loadingBuddy.color = new Color(loadingBuddyColor.r, loadingBuddyColor.g, loadingBuddyColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void InterpColors(TMP_Text text)
    {
        colorTimer += Time.deltaTime;
        text.color = Color.Lerp(ColorByIndex(nextColIndex - 1), ColorByIndex(nextColIndex), colorTimer * colorsPerSec);
        if (colorTimer >= 1 / colorsPerSec)
        {
            if (nextColIndex < colorsToInterp.Length - 1) nextColIndex++; 
            else nextColIndex = 0;
            colorTimer = 0f;
        }
    }

    void InterpColors(Image panel)
    {
        colorTimer += Time.deltaTime;
        panel.color = Color.Lerp(ColorByIndex(nextColIndex - 1), ColorByIndex(nextColIndex), colorTimer * colorsPerSec);
        if (colorTimer >= 1 / colorsPerSec)
        {
            if (nextColIndex < colorsToInterp.Length - 1) nextColIndex++;
            else nextColIndex = 0;
            colorTimer = 0f;
        }
    }

    Color ColorByIndex(int index)
    {
        if (index < 0)
        {
            return colorsToInterp[colorsToInterp.Length - 1].color;
        }
        else return colorsToInterp[index].color;
    }

    void TextInterp()
    {
        dotTimer += Time.deltaTime;
        if (dotTimer > 1 / dotsPerSec)
        {
            if (dots < maxDots)
            {
                loadingTxt.text += ".";
                dots++;
            }
            else
            {
                loadingTxt.text = ogText;
                dots = 0;
            }
            dotTimer = 0;
        }
    }

    private void OnDestroy()
    {
        Destroy(loadingBuddy.gameObject);
    }
}

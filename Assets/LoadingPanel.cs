using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    Material[] colorsToInterp;
    Image background;
    int nextColIndex = 1;
    float colorTimer = 0f, dotTimer = 0f;

    public TMP_Text loadingTxt;
    public string ogText;
    public int maxDots, dots = 0;
    public float dotsPerSec, colorsPerSec, panelScrollTime;

    // Start is called before the first frame update
    void Awake()
    {
        //if (SceneManager.GetActiveScene().buildIndex != MultiplayerSettings.multiplayerSettings.intermediateScene || SceneManager.GetActiveScene().buildIndex != MultiplayerSettings.multiplayerSettings.multiplayerScene) Destroy(this.gameObject);
        background = GetComponent<Image>();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (colorsToInterp == null) colorsToInterp = LobbyController.lc.availableMaterials.ToArray();
        
        InterpColors(loadingTxt);
        InterpColors(background);
        TextInterp();
    }

    public void DisplayLoadingPanel(bool display)
    {
        StartCoroutine(ScrollInDisplay(display));
    }

    IEnumerator ScrollInDisplay(bool toScreen)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position, endingPos;
        float time = panelScrollTime;
        if (toScreen) endingPos = Vector3.zero;
        else endingPos = new Vector3(0f, Screen.height * 2, 0f);
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, endingPos, elapsedTime / time);
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
}

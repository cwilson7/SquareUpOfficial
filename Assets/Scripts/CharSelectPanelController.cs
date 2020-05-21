using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharSelectPanelController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject charPanel;
    [SerializeField] private float offsetFromLeftEdge, percentThreshold, easing;
    private Vector3 panelLocation;
    private int panelCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        SetTransformInfo();
        for(int i = 0; i < LobbyController.lc.charAvatars.Count; i++)
        {
            GenerateCharacterPanel(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateCharacterPanel(int charID)
    {
        GameObject newPanel = Instantiate(charPanel, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2 ((charID + 0.5f) * charPanel.GetComponent<RectTransform>().rect.width, newPanel.GetComponent<RectTransform>().anchoredPosition.y);
        newPanel.GetComponent<CharPage>().ShowDetails(charID);
    }

    private void SetTransformInfo()
    {
        // Sets anchors to middle/ left of canvas
        charPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        charPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);

        panelLocation = transform.position;
        panelCounter = 1;
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
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
            } else if (percentage < 0 && (panelCounter > 1))
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
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}

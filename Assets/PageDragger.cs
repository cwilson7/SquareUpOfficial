using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageDragger : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float percentThreshold = 0.2f;
    private int currPage = 1; 
    public int totalPages;
    Vector3 pageLocation;
    float easing = 0.5f;

    private void Start()
    {
        pageLocation = transform.position;
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.y - data.position.y;
        transform.position = pageLocation - new Vector3(0, difference, 0);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.y - data.position.y) / Screen.height;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = pageLocation;
            if (percentage < 0 && currPage < totalPages)
            {
                currPage += 1;
                newLocation += new Vector3(0, Screen.height, 0);
            }
            else if (percentage > 0 && currPage > 1)
            {
                currPage -= 1;
                newLocation += new Vector3(0, -Screen.height, 0);
            }

            PageTransition(transform.position, newLocation);
            pageLocation = newLocation;
        }
        else
        {
            PageTransition(transform.position, pageLocation);
        }
    }

    void PageTransition(Vector3 oldLoc, Vector3 newLoc)
    {
        StartCoroutine(PageTransitionEnum(oldLoc, newLoc));
    }

    IEnumerator PageTransitionEnum(Vector3 startpos, Vector3 endpos)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / easing;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }

}

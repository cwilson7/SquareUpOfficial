using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticPanel : MonoBehaviour
{
    public GameObject DisplayedCharacter;
    //public GameObject HeadPnl, FacePnl, AccPnl, BodyPnl, FistsPnl;
    public CharacterInfo charInfo;

    public void Initialize()
    {
        foreach (CosmeticOptionsHandler option in GetComponentsInChildren<CosmeticOptionsHandler>())
        {
            option.info = charInfo;
            option.LoadGrid();
            option.gameObject.SetActive(false);
        }
    }

    private void StartPanel(GameObject pnl)
    {
        pnl.GetComponent<CosmeticOptionsHandler>().LoadGrid();
    }

    public void OpenPanel(GameObject pnl)
    {
        pnl.SetActive(true);
    }

    public void ClosePanel(GameObject pnl)
    {
        if (pnl == this.gameObject)
        {
            GetComponentInParent<ShopPanel>().CloseCosmeticsPanel();
        }
        else pnl.SetActive(false);
    }
}

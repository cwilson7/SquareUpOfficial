using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;

public class SelectCharacterPanelButton : MonoBehaviour
{
    public GameObject character;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(MoveToPanel);
    }

    private void MoveToPanel()
    {
        List<GameObject> panels = ShopController.Instance.characterPanels;
        for (int i = 0; i < panels.Count; i ++)
        {
            if (panels[i].GetComponent<ShopPanel>().Character == character)
            {
                panels[i].GetComponent<ShopPanel>().Display();
                Utils.FindParentWithClass<MainPanel>(transform).gameObject.SetActive(false);
                break;
            }
        }
    }
}

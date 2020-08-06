using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using TMPro;

public class CosmeticOptionsHandler : MonoBehaviour
{
    private Scrollbar scrollbar;
    private GridLayoutGroup cosmeticsGrid;
    public int options_per_row, option_height;
    [SerializeField] private CosmeticType cosmeticType;
    [SerializeField] private GameObject cosmeticOptionPrefab;

    [HideInInspector] public CharacterInfo info;
    [HideInInspector] public GameObject displayedCharacter;

    public void LoadGrid()
    {
        int totalItemCount = 0;
        scrollbar = GetComponentInChildren<Scrollbar>();
        cosmeticsGrid = GetComponentInChildren<GridLayoutGroup>();
        RectTransform gridRect = cosmeticsGrid.gameObject.GetComponent<RectTransform>();

        cosmeticsGrid.cellSize = new Vector2(gridRect.rect.width / options_per_row, option_height);

        foreach (CosmeticItem item in info.cosmetics)
        {
            if (item.type == cosmeticType)
            {
                GameObject optionGO = Instantiate(cosmeticOptionPrefab, cosmeticsGrid.transform);
                optionGO.GetComponent<CosmeticOptionButton>().option = item;
                optionGO.GetComponent<CosmeticOptionButton>().avatar = displayedCharacter.GetComponent<AvatarCharacteristics>();
                
                if (item.status == Status.Locked) optionGO.GetComponentInChildren<TMP_Text>().text = "Locked.";
                else optionGO.GetComponentInChildren<TMP_Text>().text = item.name;
                totalItemCount++;
            }
        }

        int rows = totalItemCount / options_per_row;
        if (rows * option_height > gridRect.rect.height)
        {            
            int rowsOnPage = Mathf.RoundToInt(gridRect.rect.height) / option_height;
            int leftoverRows = rows - rowsOnPage;
            scrollbar.numberOfSteps = leftoverRows;
            scrollbar.size = rows / scrollbar.numberOfSteps;
        }
        else scrollbar.enabled = false;
    }

    public void SliderFunction()
    {

    }
}

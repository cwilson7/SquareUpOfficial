using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class CosmeticOptionsHandler : MonoBehaviour
{
    private GridLayoutGroup cosmeticsGrid;
    public int options_per_row, options_per_column;
    [SerializeField] private CosmeticType cosmeticType;
    [SerializeField] private GameObject cosmeticOptionPrefab;

    [HideInInspector] public CharacterInfo info;
    [HideInInspector] public GameObject displayedCharacter;

    public void LoadGrid()
    {
        int totalItems = 0;
        cosmeticsGrid = GetComponentInChildren<GridLayoutGroup>();
        RectTransform gridRect = cosmeticsGrid.gameObject.GetComponent<RectTransform>();

        cosmeticsGrid.cellSize = new Vector2(gridRect.rect.width / options_per_row, gridRect.rect.height / options_per_column);
        cosmeticsGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cosmeticsGrid.constraintCount = options_per_row;

        foreach (CosmeticItem item in info.cosmetics)
        {
            if (item.type == cosmeticType)
            {
                GameObject optionGO = Instantiate(cosmeticOptionPrefab, cosmeticsGrid.transform);
                optionGO.GetComponent<CosmeticOptionButton>().option = item;
                optionGO.GetComponent<CosmeticOptionButton>().avatar = displayedCharacter.GetComponent<AvatarCharacteristics>();
                
                if (item.status == Status.Locked) optionGO.GetComponentInChildren<TMP_Text>().text = "Locked.";
                else optionGO.GetComponentInChildren<TMP_Text>().text = item.name;

                totalItems++;
            }
        }

        cosmeticsGrid.gameObject.GetComponent<PageDragger>().totalPages = Mathf.CeilToInt(((float)totalItems / ((float)options_per_row * (float)options_per_column)));
    }
}

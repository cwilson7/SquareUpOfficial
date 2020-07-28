using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using TMPro;

public class CosmeticOptionsHandler : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cosmeticsGrid;
    [SerializeField] private GameObject cosmeticOptionPrefab;

    [HideInInspector] public CharacterInfo info;
    [SerializeField] private CosmeticType cosmeticType;
    [HideInInspector] public GameObject displayedCharacter;

    public void LoadGrid()
    {
        foreach (CosmeticItem item in info.cosmetics)
        {
            if (item.type == cosmeticType)
            {
                GameObject optionGO = Instantiate(cosmeticOptionPrefab, cosmeticsGrid.transform);
                optionGO.GetComponent<CosmeticOptionButton>().option = item;
                optionGO.GetComponent<CosmeticOptionButton>().avatar = displayedCharacter.GetComponent<AvatarCharacteristics>();

                if (item.status == Status.Locked) optionGO.GetComponentInChildren<TMP_Text>().text = "Locked.";
                else optionGO.GetComponentInChildren<TMP_Text>().text = item.name;
            }
        }
    }

    public void SliderFunction()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticOptionButton : MonoBehaviour
{
    public CosmeticItem option;
    public AvatarCharacteristics avatar;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectOption);
    }

    void SelectOption()
    {
        avatar.EquipCosmetic(option);
    }
}

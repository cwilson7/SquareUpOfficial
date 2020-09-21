﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using TMPro;

public class UnlockButton : MonoBehaviour
{
    public void Start()
    {
        Status isLocked;

        if (GetComponent<CosmeticOptionButton>() != null)
        {
            isLocked = GetComponent<CosmeticOptionButton>().option.status;
            if (isLocked == Status.Locked) GetComponent<Button>().onClick.AddListener(UnlockCosmetic);
        }
        else
        {
            isLocked = ProgressionSystem.CharacterData(GetComponentInParent<ShopPanel>().Character.GetComponent<AvatarCharacteristics>().info).status;
            if (isLocked == Status.Locked) GetComponent<Button>().onClick.AddListener(UnlockCharacter);
        }
        if (isLocked == Status.Locked)
        {
            GetComponentInChildren<TMP_Text>().text = "UNLOCK";
        }
    }

    public void UnlockCharacter()
    {
        List<CharacterInfo> newInfos = new List<CharacterInfo>();
        List<Currency> newWallet = new List<Currency>();

        AvatarCharacteristics AC = GetComponentInParent<ShopPanel>().Character.GetComponent<AvatarCharacteristics>();
        CharacterInfo info = ProgressionSystem.CharacterData(AC.info);
        
        foreach (Currency currency in ProgressionSystem.playerData.wallet)
        {
            if (info.cost.type == currency.type)
            {
                if (currency.Quantity >= info.cost.Quantity)
                {
                    currency.Quantity -= info.cost.Quantity;
                    info.status = Status.Unlocked;
                    GetComponentInChildren<TMP_Text>().text = info.characterName;
                }
                else
                {
                    Debug.Log("Not enough cash bruh");
                    return;
                }
            }
            newWallet.Add(currency);
        }

        foreach (CharacterInfo ci in ProgressionSystem.playerData.Characters)
        {
            if (info.characterName == ci.characterName)
            {
                newInfos.Add(info);
            }
            else newInfos.Add(ci);
        }

        ProgressionSystem.playerData.wallet = newWallet.ToArray();
        ProgressionSystem.playerData.Characters = newInfos;
        ProgressionSystem.SaveData();
    }

    public void UnlockCosmetic()
    {
        CosmeticOptionButton option = GetComponent<CosmeticOptionButton>();
        AvatarCharacteristics AC = option.avatar;
        CosmeticItem desiredItem = option.option;

        List<CharacterInfo> newInfos = new List<CharacterInfo>();
        List<Currency> newWallet = new List<Currency>();

        CharacterInfo info = ProgressionSystem.CharacterData(AC.info);
        List<CosmeticItem> newItems = new List<CosmeticItem>();
        List<CosmeticItem> allItems = info.cosmetics;

        Currency[] bigpp = { new Currency(Money.SquareBucks, 500), new Currency(Money.CubeCoins, 50) };

        foreach (CosmeticItem item in allItems)
        {
            if (desiredItem.name == item.name)
            {
                foreach (Currency currency in ProgressionSystem.playerData.wallet)
                {
                    if (item.cost.type == currency.type)
                    {
                        if (currency.Quantity >= item.cost.Quantity)
                        {
                            currency.Quantity -= item.cost.Quantity;
                            item.status = Status.Unlocked;
                            GetComponentInChildren<TMP_Text>().text = item.name;
                        }
                        else
                        {
                            Debug.Log("Not enough cash bruh");
                        }                        
                    }
                    newWallet.Add(currency);
                }
            }
            newItems.Add(item);
        }

        info.cosmetics = newItems;

        foreach (CharacterInfo ci in ProgressionSystem.playerData.Characters)
        {
            if (info.characterName == ci.characterName)
            {
                newInfos.Add(info);
            }
            else newInfos.Add(ci);
        }

        ProgressionSystem.playerData.wallet = newWallet.ToArray();
        ProgressionSystem.playerData.Characters = newInfos;
        ProgressionSystem.SaveData();
    }
}

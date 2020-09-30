using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerRewards
{
    public static void AddCurrency(PlayerData playerData, Money type, int amount)
    {
        foreach (Currency currency in playerData.wallet)
        {
            if (currency.type == type)
            {
                playerData.UpdateWallet(type, amount);
                break;
            }
        }
    }
}

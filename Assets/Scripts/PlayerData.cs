using CustomUtilities;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int wins, totalGames;
    public Currency coins, bucks;
    public Currency[] wallet;
    public List<CharacterInfo> characters;
    public List<CustomEffect> availableEffects;
    public string crownPath, myCrownName;

    public int SquareBucks
    {
        get
        {
            if (bucks == null)
            {
                bucks = new Currency(Money.SquareBucks, 69);
                Debug.Log("bucks r null");
            }
            return bucks.Quantity;
        }
        set
        {
            bucks.Quantity = value;
        }
    }
    public int CubeCoins
    {
        get
        {
            if (coins == null)
            {
                coins = new Currency(Money.CubeCoins, 69);
                Debug.Log("coins r null");
            }
            return coins.Quantity;
        }
        set
        {
            coins.Quantity = value;
        }
    }
    public int Wins
    {
        get
        {
            return wins;
        }
        set
        {
            wins = value;
        }
    }
    public int TotalGames
    {
        get
        {
            return totalGames;
        }
        set
        {
            totalGames = value;
        }
    }
    public List<CharacterInfo> Characters
    {
        get
        {
            return characters;
        }
        set
        {
            characters = value;
        }
    }
    public List<CustomEffect> AvailableEffects
    {
        get
        {
            return availableEffects;
        }
        set
        {
            availableEffects = value;
        }
    }

    public void UpdateWallet(Money type, int amount)
    {
        if (type == Money.SquareBucks) SquareBucks += amount;
        else if (type == Money.CubeCoins) CubeCoins += amount;

        wallet = new Currency[2]{ new Currency(Money.SquareBucks, SquareBucks), new Currency(Money.CubeCoins, CubeCoins) };
    }

    public PlayerData(int _squareBucks, int _cubeCoins, int _wins, int _totalGames, List<CharacterInfo> _characters, List<CustomEffect> _availableEffects)
    {
        bucks = new Currency(Money.SquareBucks, _squareBucks);
        coins = new Currency(Money.CubeCoins, _cubeCoins);
        crownPath = "PhotonPrefabs/Cosmetics/Crowns/";
        myCrownName = "Default";
        wallet = new Currency[2] { bucks, coins };
        wins = _wins;
        totalGames = _totalGames;
        characters = _characters;
        availableEffects = _availableEffects;
    }
}

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
    public string crownPath, myCrownName;
    public CrownData[] crownDataArray;

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

    public void UpdateWallet(Money type, int amount)
    {
        if (type == Money.SquareBucks) SquareBucks += amount;
        else if (type == Money.CubeCoins) CubeCoins += amount;

        wallet = new Currency[2]{ new Currency(Money.SquareBucks, SquareBucks), new Currency(Money.CubeCoins, CubeCoins) };
    }

    void CreateNewCrownArray(string path)
    {
        List<CrownData> crownDatas = new List<CrownData>();
        GameObject[] crowns = Resources.LoadAll<GameObject>(path);
        for (int i = 0; i < crowns.Length; i++)
        {
            string objectName = crowns[i].name;
            Status objectStatus = Status.Locked;
            if (objectName == "Default") objectStatus = Status.Unlocked;
            CrownData data = new CrownData(objectName, objectStatus);
            crownDatas.Add(data);
        }
        crownDataArray = crownDatas.ToArray();
        myCrownName = "Default";
    }

    public PlayerData(int _squareBucks, int _cubeCoins, int _wins, int _totalGames, List<CharacterInfo> _characters)
    {
        bucks = new Currency(Money.SquareBucks, _squareBucks);
        coins = new Currency(Money.CubeCoins, _cubeCoins);
        crownPath = "PhotonPrefabs/Cosmetics/Crowns/";
        CreateNewCrownArray(crownPath);
        wallet = new Currency[2] { bucks, coins };
        wins = _wins;
        totalGames = _totalGames;
        characters = _characters;
    }
}

using CustomUtilities;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int squareBucks, cubeCoins, wins, totalGames;
    public List<CharacterInfo> characters;
    public List<CustomEffect> availableEffects; 

    public int SquareBucks
    {
        get
        {
            return squareBucks;
        }
        set
        {
            squareBucks = value;
        }
    }
    public int CubeCoins
    {
        get
        {
            return cubeCoins;
        }
        set
        {
            cubeCoins = value;
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

    public PlayerData(int _squareBucks, int _cubeCoins, int _wins, int _totalGames, List<CharacterInfo> _characters, List<CustomEffect> _availableEffects)
    {
        squareBucks = _squareBucks;
        cubeCoins = _cubeCoins;
        wins = _wins;
        totalGames = _totalGames;
        characters = _characters;
        availableEffects = _availableEffects;
    }
}

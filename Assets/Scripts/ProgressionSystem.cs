using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour
{
    public static PlayerData playerData;  

    private void OnEnable()
    {
        LoadData();
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }

    public static void SaveData()
    {
        Debug.Log("saving game.");
        //SaveState.SaveInformation(playerData);
        string dataString = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("GameData", dataString);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //SaveData();
    }
    
    public void ReloadCosmetics()
    {
        List<GameObject> Characters = new List<GameObject>();
        Utils.PopulateList<GameObject>(Characters, "PhotonPrefabs/CharacterAvatars");
        foreach (GameObject _char in Characters)
        {
            AvatarCharacteristics AC = _char.GetComponent<AvatarCharacteristics>();
            CharacterInfo info = AC.info;
            info.cosmetics = AC.LoadCosmetics();
        }
    }

    void LoadData()
    {
        if (!PlayerPrefs.HasKey("GameData"))
        {
            Debug.Log("setting up new game");
            playerData = new PlayerData(500, 5, 0, 0, NewCharacterInfoList(), new List<CustomEffect>());
            string dataString = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString("GameData", dataString);
        }
        else
        {
            Debug.Log("loading previous save");
            string dataString = PlayerPrefs.GetString("GameData");
            playerData = JsonUtility.FromJson<PlayerData>(dataString);
        }

        /*
        if (true) //if first load
        {
            playerData = new PlayerData(500, 5, 0, 0, NewCharacterInfoList(), new List<CustomEffect>());           
            SaveState.SaveInformation(playerData);
        }
        else
        {
            playerData = SaveState.LoadInformation();
        }
        */
    }

    List<CharacterInfo> NewCharacterInfoList()
    {
        List<CharacterInfo> newGameList = new List<CharacterInfo>();
        List<GameObject> Characters = new List<GameObject>();
        Utils.PopulateList<GameObject>(Characters, "PhotonPrefabs/CharacterAvatars");
        foreach (GameObject _char in Characters)
        {
            AvatarCharacteristics AC = _char.GetComponent<AvatarCharacteristics>();
            CharacterInfo info = AC.info;
            info.cosmetics = AC.LoadCosmetics();
            info.currentSet = new CosmeticSet();
            GameObject defaultFist =  AC.defaultFist;
            CosmeticData money = defaultFist.GetComponent<CosmeticData>();
            CosmeticItem defaultFistItem = new CosmeticItem(CosmeticType.Fist, defaultFist, Status.Unlocked, money.type, money.value);
            info.currentSet.cosmetics.Add(CosmeticType.Fist, defaultFistItem);
            info.model = _char;
            //new CharacterInfo(_char.name, Status.Unlocked, AC.MyLevels, AC.LoadCosmetics(), new CosmeticSet());
            newGameList.Add(info);
        }
        return newGameList;
    }

    public static CharacterInfo CharacterData(CharacterInfo info)
    {
        CharacterInfo returnInfo = info;
        foreach (CharacterInfo ci in playerData.Characters)
        {
            if (info.characterName == ci.characterName)
            {
                returnInfo = ci;
                break;
            }
        }
        return returnInfo;
    }

    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

[System.Serializable]
public class CharacterInfo
{
    public GameObject model;
    public string characterName;
    public Status status;
    public List<Level> associatedLevels;
    public List<CosmeticItem> cosmetics;
    public CosmeticSet currentSet;
    public Currency cost;

    public CharacterInfo(string _name, Status _status, List<Level> _associatedLevels, List<CosmeticItem> _cosmetics, CosmeticSet set)
    {
        characterName = _name;
        status = _status;
        associatedLevels = _associatedLevels;
        cosmetics = _cosmetics;
        currentSet = set;
        CosmeticData money = model.GetComponent<CosmeticData>();
        cost.type = money.type;
        cost.Quantity = money.value;
    }
}

public class CustomEffect : MonoBehaviour
{
    public Status status;

}

[System.Serializable]
public static class SaveState
{   
    public static void SaveInformation (PlayerData _playerData)
    {
        string serializedPlayerData = JsonUtility.ToJson(_playerData);
        CloudSaveHandler.SaveStringProgressSystem(serializedPlayerData);
    }

    public static PlayerData LoadInformation()
    {
        PlayerData data = (PlayerData)JsonUtility.FromJson(CloudSaveHandler.PlayerInformation(), typeof(PlayerData));
        return data;
    }
}

[System.Serializable]
public enum Status
{
    Locked,
    Unlocked
}

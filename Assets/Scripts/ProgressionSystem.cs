using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour
{
    public static PlayerData playerData;
    static string testPrefString = "bonkssss";

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
        PlayerPrefs.SetString(testPrefString, dataString);
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
        if (!PlayerPrefs.HasKey(testPrefString))
        {
            Debug.Log("setting up new game");
            playerData = new PlayerData(500, 5, 0, 0, NewCharacterInfoList(), new List<CustomEffect>());
            string dataString = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString(testPrefString, dataString);
        }
        else
        {
            Debug.Log("loading previous save");
            string dataString = PlayerPrefs.GetString(testPrefString);
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

    public void UnlockNewRandomCrown()
    {
        CrownData[] crownDatas = playerData.crownDataArray;
        List<string> lockedCrownNames = new List<string>();
        for (int i = 0; i < crownDatas.Length; i++) 
        {
            if (crownDatas[i].status == Status.Locked)
            {
                lockedCrownNames.Add(crownDatas[i].name);
            }
        }

        if (lockedCrownNames.Count == 0)
        {
            Debug.Log("All crowns already unlocked");
            return;
        }

        string crownName = lockedCrownNames[Random.Range(0, lockedCrownNames.Count)];
        for (int j = 0; j < crownDatas.Length; j++)
        {
            if (crownName == crownDatas[j].name)
            {
                crownDatas[j].status = Status.Unlocked;
                break;
            }
        }
        playerData.crownDataArray = crownDatas;
        SaveData();
    }

    public void UnlockNewCrown(string crownName)
    {
        CrownData[] crownDatas = playerData.crownDataArray;
        for (int i = 0; i < crownDatas.Length; i++)
        {
            if (crownName == crownDatas[i].name)
            {
                crownDatas[i].status = Status.Unlocked;
                break;
            }
        }
        playerData.crownDataArray = crownDatas;
        SaveData();
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

[System.Serializable]
public struct CrownData
{
    public string name;
    public Status status;

    public CrownData(string _name, Status _status)
    {
        name = _name;
        status = _status;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour
{
    public static PlayerData playerData;
    static string testPrefString = "dataabro0o0";
    bool checkForUpdate = true;

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
        if (!PlayerPrefs.HasKey(testPrefString)) //if first load
        {
            Debug.Log("setting up new game");
            playerData = new PlayerData(500, 5, 0, 0, NewCharacterInfoList());
            string dataString = JsonUtility.ToJson(playerData);
            PlayerPrefs.SetString(testPrefString, dataString);
        }
        else
        {
            Debug.Log("loading previous save");
            string dataString = PlayerPrefs.GetString(testPrefString);
            playerData = JsonUtility.FromJson<PlayerData>(dataString);
        }
        if (checkForUpdate) CheckForUpdates();

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

    void CheckForUpdates()
    {
        Debug.Log("Checking for updates...");
        
        GameObject[] crownsInFile = Resources.LoadAll<GameObject>(playerData.crownPath);
        GameObject[] charactersInFile = Resources.LoadAll<GameObject>("PhotonPrefabs/CharacterAvatars");
        bool missingCharacters = playerData.characters.Count != charactersInFile.Length;

        if (playerData.crownDataArray.Length != crownsInFile.Length)
        {
            // need to load in missing crowns
            Debug.Log("Saved data is missing a crown.");
            List<CrownData> crownDatas = playerData.crownDataArray.ToList();
            for (int i = 0; i < crownsInFile.Length; i++)
            {
                string fileCrownName = crownsInFile[i].name;
                if(!crownDatas.Contains(new CrownData(fileCrownName, Status.Locked)) && !crownDatas.Contains(new CrownData(fileCrownName, Status.Unlocked)))
                {
                    Debug.Log("Adding new crown to saved data: " + fileCrownName);
                    crownDatas.Add(new CrownData(fileCrownName, Status.Locked));
                }
            }
            playerData.crownDataArray = crownDatas.ToArray();
            Debug.Log("Saved crowns updated.");
        }
        foreach (GameObject model in charactersInFile)
        {
            AvatarCharacteristics AC = model.GetComponent<AvatarCharacteristics>();
            CharacterInfo ci = AC.info;           
            string path = AC.CosmeticFolder;
            CosmeticLoader[] folders = AC.folders;
            List<(GameObject, CosmeticType)> items = new List<(GameObject, CosmeticType)>();
            if (missingCharacters)
            {
                Debug.Log("Saved data is missing a character. Attempting to identify missing character...");
                // load in missing characters
                bool modelInSavedData = false;
                for (int i = 0; i < playerData.characters.Count; i++)
                {
                    if (playerData.characters[i].characterName == model.name) modelInSavedData = true;
                }
                if (!modelInSavedData)
                {
                    Debug.Log(model.name + " is missing from saved data.");
                    List<CharacterInfo> newList = playerData.characters;
                    CharacterInfo info = AC.info;
                    info.cosmetics = AC.LoadCosmetics();
                    info.currentSet = new CosmeticSet();
                    GameObject defaultFist = AC.defaultFist;
                    CosmeticData money = defaultFist.GetComponent<CosmeticData>();
                    CosmeticItem defaultFistItem = new CosmeticItem(CosmeticType.Fist, defaultFist, Status.Unlocked, money.type, money.value);
                    info.currentSet.cosmetics.Add(CosmeticType.Fist, defaultFistItem);
                    info.model = model;
                    newList.Add(info);
                    playerData.characters = newList;
                    missingCharacters = playerData.characters.Count != charactersInFile.Length;
                    Debug.Log(model.name + " has been added to saved data.");
                }
            }
            foreach (CosmeticLoader folder in folders)
            {
                GameObject[] folderItems = Resources.LoadAll<GameObject>("PhotonPrefabs/Cosmetics/" + path + '/' + folder.FolderName);
                foreach (GameObject folderItem in folderItems)
                {
                    items.Add((folderItem, folder.cosmeticType));
                }
            }
            // find which character this is in the list
            List<CosmeticItem> itemsInSavedData = new List<CosmeticItem>();
            int indexOfChar = -1;
            for(int i = 0; i < playerData.characters.Count; i++)
            {
                if (model.name == playerData.characters[i].characterName)
                {
                    itemsInSavedData = playerData.characters[i].cosmetics;
                    indexOfChar = i;
                    break;
                }
            }
            bool itemsMissing = items.Count != itemsInSavedData.Count;
            if (itemsMissing)
            {
                Debug.Log("An item is missing from " + playerData.characters[indexOfChar].characterName + "'s cosmetics.");
                // need to load in missing cosmetics
                foreach ((GameObject, CosmeticType) fileItem in items)
                {
                    bool isContainedInSavedData = false;
                    foreach (CosmeticItem savedItem in itemsInSavedData)
                    {
                        if (fileItem.Item1.name == savedItem.name) isContainedInSavedData = true;
                    }
                    if (!isContainedInSavedData)
                    {
                        Debug.Log("Missing item identified: " + fileItem.Item1.name);
                        List<CosmeticItem> newList = itemsInSavedData;
                        CosmeticData money = model.GetComponent<CosmeticData>();
                        newList.Add(new CosmeticItem(fileItem.Item2, fileItem.Item1, Status.Locked, money.type, money.value));
                        playerData.characters[indexOfChar].cosmetics = newList;
                        itemsMissing = items.Count != playerData.characters[indexOfChar].cosmetics.Count;
                        Debug.Log(fileItem.Item1.name + " has been added to " + playerData.characters[indexOfChar].characterName + "'s cosmetics.");
                        if (!itemsMissing) break;
                    }
                }
            }
        }
        Debug.Log("Up to date.");
        SaveData();
        checkForUpdate = false;
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
    public CustomEffect[] defaultEffects;
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
        if (money.associatedEffects.Length != 3)
        {
            Debug.Log(money.associatedEffects.Length + " default effects counted for " + characterName + ". There should be 3. Edit CosmeticData script on prefab.");
        }
        defaultEffects = money.associatedEffects;
    }

    public string PathOfDefaultEffect(EffectType _type)
    {
        string path = null;
        for (int i = 0; i < defaultEffects.Length; i++)
        {
            if (defaultEffects[i].type == _type)
            {
                path = defaultEffects[i].filePath;
            }
        }
        return path;
    }
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

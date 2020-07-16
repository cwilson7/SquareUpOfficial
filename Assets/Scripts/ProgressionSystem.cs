using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using CustomUtilities;
using System;
using System.Runtime.InteropServices;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance;
    public int SquareBucks;
    public Hashtable Characters;
    public List<CustomEffect> AvailableEffects;
    private void Awake()
    {
        if (ProgressionSystem.Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        SetupSaveState();
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetupSaveState()
    {
        PlayerData LoadedInfo = null;//SaveState.LoadInformation();
        Instance = this;
        if (LoadedInfo == null) this.NewSave();
        else CopyLoadedInfo(LoadedInfo);

        //SaveState.SaveInformation(this);
        //UnlockButton.ProgressionSystemChange += SaveState.SaveInformation;
        //ShopController.SaveGane += SaveState.SaveInformation;
    }

    public ProgressionSystem()
    {
        AvailableEffects = new List<CustomEffect>();
        Characters = new Hashtable();
    }

    void NewSave()
    {
        Characters = NewCharacterInfoHash();
        SquareBucks = 500;
    }

    void CopyLoadedInfo(PlayerData LoadedInfo)
    {
        this.SquareBucks = LoadedInfo.progressSystem.SquareBucks;
        this.AvailableEffects = LoadedInfo.progressSystem.AvailableEffects;
        this.Characters = LoadedInfo.progressSystem.Characters;
    }

    Hashtable NewCharacterInfoHash()
    {
        Hashtable returnHash = new Hashtable();
        List<GameObject> Characters = new List<GameObject>();
        Utils.PopulateList<GameObject>(Characters, "PhotonPrefabs/CharacterAvatars");
        foreach (GameObject _char in Characters)
        {
            CharacterInfo info = new CharacterInfo(_char.name, Status.Unlocked, _char.GetComponent<AvatarCharacteristics>().MyLevels, _char.GetComponent<AvatarCharacteristics>().cosmetics);
            returnHash.Add(info.characterName, info);
        }
        return returnHash;
    }
}

[System.Serializable]
public class CharacterInfo
{
    public string characterName;
    public Status status;
    public List<Level> associatedLevels;
    public List<CosmeticItem> cosmetics;

    public CharacterInfo(string _name, Status _status, List<Level> _associatedLevels, List<CosmeticItem> _cosmetics)
    {
        characterName = _name;
        status = _status;
        associatedLevels = _associatedLevels;
        cosmetics = _cosmetics;
    }
}

public class CustomEffect : MonoBehaviour
{
    public Status status;

}

[System.Serializable]
public class PlayerData
{
    public ProgressionSystem progressSystem;

    public PlayerData(ProgressionSystem _PS)
    {
        progressSystem = _PS;
    }
}

[System.Serializable]
public static class SaveState
{
    
    public static void SaveInformation (ProgressionSystem _ps)
    {
        Debug.Log("progression state saved");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Progress.balls";
        System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create);

        PlayerData data = new PlayerData(ProgressionSystem.Instance);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadInformation()
    {
        string path = Application.persistentDataPath + "/Progress.balls";
        if (System.IO.File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open);

            PlayerData data = (PlayerData)formatter.Deserialize(stream);
            
            stream.Close();

            return data;
        } 
        else
        {
            Debug.Log("file not found: " + path);
            return null;
        }
    }
}

[System.Serializable]
public enum Status
{
    Locked,
    Unlocked
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using CustomUtilities;
using System;
using System.Runtime.InteropServices;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour, ISerializationCallbackReceiver
{
    [Header("Characters Info")]
    public List<string> _keys;
    public List<CharacterInfo> _values;

    public static ProgressionSystem Instance;
    public int SquareBucks;
    //public Hashtable Characters;
    public Dictionary<string, CharacterInfo> Characters;
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
        Characters = new Dictionary<string, CharacterInfo>();//new Hashtable();
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

    //Hashtable
    Dictionary<string, CharacterInfo>  NewCharacterInfoHash()
    {
        //Hashtable returnHash = new Hashtable();
        Dictionary<string, CharacterInfo> returnHash = new Dictionary<string, CharacterInfo>();
        List<GameObject> Characters = new List<GameObject>();
        Utils.PopulateList<GameObject>(Characters, "PhotonPrefabs/CharacterAvatars");
        foreach (GameObject _char in Characters)
        {
            AvatarCharacteristics AC = _char.GetComponent<AvatarCharacteristics>();
            CharacterInfo info = AC.info;
            info.cosmetics = AC.LoadCosmetics();
            info.currentSet = new CosmeticSet();
            info.model = _char;
            //new CharacterInfo(_char.name, Status.Unlocked, AC.MyLevels, AC.LoadCosmetics(), new CosmeticSet());
            returnHash.Add(info.characterName/*_char.name*/, info);
        }
        return returnHash;
    }

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in Characters)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Characters = new Dictionary<string, CharacterInfo>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            Characters.Add(_keys[i], _values[i]);
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

    public CharacterInfo(string _name, Status _status, List<Level> _associatedLevels, List<CosmeticItem> _cosmetics, CosmeticSet set)
    {
        characterName = _name;
        status = _status;
        associatedLevels = _associatedLevels;
        cosmetics = _cosmetics;
        currentSet = set;
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

public enum Direction
{
    ToCenter,
    FromCenter
}

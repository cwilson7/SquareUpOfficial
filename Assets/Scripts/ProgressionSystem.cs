using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using CustomUtilities;

[System.Serializable]
public class ProgressionSystem : MonoBehaviour
{
    public static ProgressionSystem Instance;
    public int SquareBucks;
    public List<CharacterInfo> AvailableCharacters;
    public List<CustomEffect> AvailableEffects;

    private void Awake()
    {
        PlayerData LoadedInfo = SaveState.LoadInformation();
        Instance = this;
        if (LoadedInfo == null) this.NewSave();
        else CopyLoadedInfo(LoadedInfo);
        DontDestroyOnLoad(this.gameObject);
    }

    public ProgressionSystem()
    {
        AvailableCharacters = new List<CharacterInfo>();
        AvailableEffects = new List<CustomEffect>();
    }

    void NewSave()
    {
        AvailableCharacters = NewCharacterInfoList();
        SquareBucks = 500;
    }

    void CopyLoadedInfo(PlayerData LoadedInfo)
    {
        this.AvailableCharacters = LoadedInfo.progressSystem.AvailableCharacters;
        this.SquareBucks = LoadedInfo.progressSystem.SquareBucks;
        this.AvailableEffects = LoadedInfo.progressSystem.AvailableEffects;
    }

    List<CharacterInfo> NewCharacterInfoList()
    {
        List<CharacterInfo> ret = new List<CharacterInfo>();
        List<GameObject> Characters = new List<GameObject>();
        Utils.PopulateList<GameObject>(Characters, "PhotonPrefabs/CharacterAvatars");
        foreach (GameObject _char in Characters)
        {
            CharacterInfo info = new CharacterInfo(_char.name, Status.Unlocked, _char.GetComponent<AvatarCharacteristics>().MyLevel, _char.GetComponent<AvatarCharacteristics>().skins);
            ret.Add(info);
        }
        return ret;
    }
}

[System.Serializable]
public class CharacterInfo
{
    public string characterName;
    public Status status;
    public Level associatedLevel;
    public List<CharacterSkin> skins;

    public CharacterInfo(string _name, Status _status, Level _associatedLevel, List<CharacterSkin> _skins)
    {
        characterName = _name;
        status = _status;
        associatedLevel = _associatedLevel;
        skins = _skins;
    }
}

[System.Serializable]
public class CharacterSkin
{
    public string skinName;
    public Status status;
    public GameObject model;
}

public class CustomEffect : MonoBehaviour
{
    public Status status;

}

public class PlayerData
{
    public ProgressionSystem progressSystem;

    public PlayerData(ProgressionSystem _PS)
    {
        progressSystem = _PS;
    }
}

public static class SaveState
{
    
    public static void SaveInformation (ProgressionSystem _ps)
    {
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
            stream.Close();

            return (PlayerData)formatter.Deserialize(stream);
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

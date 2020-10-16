using ExifLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CosmeticItem
{
    public string name;
    public CosmeticType type;
    public GameObject model;
    public Status status;
    public GameObject referencedObject;
    public GameObject[] referencedObjects;
    public Currency cost;

    // check if character has
    public CustomEffect[] effects;


    public CosmeticItem(CosmeticType _type, GameObject _model, Status _status, Money currencyType, int value)
    {
        type = _type;
        model = _model;
        if (model != null) name = _model.name;
        status = _status;
        cost = new Currency(currencyType, value);
    }

    public CosmeticItem(CosmeticType _type, GameObject _model, Status _status, Money currencyType, int value, CustomEffect[] _effects)
    {
        type = _type;
        model = _model;
        if (model != null) name = _model.name;
        status = _status;
        cost = new Currency(currencyType, value);
        effects = _effects;
    }

    public string PathOfEffect(EffectType _type)
    {
        string path = null;
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].type == _type)
            {
                path = effects[i].filePath;
            }
        }
        return path;
    }

    public bool IsNull()
    {
        return model == null;
    }
}

[System.Serializable]
public enum CosmeticType
{
    Head,
    Face,
    Accessory,
    Fist,
    Body
}

[System.Serializable]
public class CosmeticLoader
{
    public string FolderName;
    public CosmeticType cosmeticType;

    public CosmeticLoader(string path, CosmeticType _type)
    {
        FolderName = path;
        cosmeticType = _type;
    }
}

[System.Serializable]
public class CustomEffect
{
    //public Status status;
    public string filePath;
    public EffectType type;

    public CustomEffect(string path, EffectType _type)
    {
        filePath = path;
        type = _type;
    }
}

[System.Serializable]
public enum EffectType
{
    Death,
    Spawn,
    Ability
}

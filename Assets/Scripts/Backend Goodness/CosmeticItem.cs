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

    public CosmeticItem(CosmeticType _type, GameObject _model, Status _status)
    {
        name = _model.name;
        type = _type;
        model = _model;
        status = _status;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CosmeticItem
{
    public CosmeticType type;
    public GameObject model;

    public CosmeticItem(CosmeticType _type, GameObject _model)
    {
        type = _type;
        model = _model;
    }
}

public enum CosmeticType
{
    Head,
    Face,
    Accessory,
    Fist,
    Body
}

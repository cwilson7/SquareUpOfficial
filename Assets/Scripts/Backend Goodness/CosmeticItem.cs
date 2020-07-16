using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CosmeticItem
{
    public CosmeticType type;
    public GameObject model;
    public Transform location;

    public CosmeticItem(CosmeticType _type, GameObject _model, Transform _location)
    {
        type = _type;
        model = _model;
        location = _location;
    }
}

public enum CosmeticType
{
    Head,
    Face, 
    Fist,
    Foot, 
    Body
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using CustomUtilities;
using System;

public class AvatarCharacteristics : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterInfo info;
    public int[] indexesOfMaterial;
    public VideoClip myDemo;

    [Header("Cosmetic Folder Information")]
    [Tooltip("Enter only the name of specified character's cosmetics folder")]
    [SerializeField] private string CosmeticFolder;

    [Tooltip("Enter name of folder and type of cosmetics that that folder houses")]
    [SerializeField] private CosmeticLoader[] folders;

    //only called on new save
    public List<CosmeticItem> LoadCosmetics()
    {
        List<CosmeticItem> ReturnList = new List<CosmeticItem>();

        for (int i = 0; i < folders.Length; i++)
        {
            List<GameObject> models = new List<GameObject>();
            string path = "PhotonPrefabs/Cosmetics/" + CosmeticFolder + "/" + folders[i].FolderName;
            Utils.PopulateList<GameObject>(models, path);
            foreach (GameObject model in models)
            {
                //eventually change status to Locked
                ReturnList.Add(new CosmeticItem(folders[i].cosmeticType, model, Status.Unlocked));
            }
        } 
        
        return ReturnList;
    }

    public void SetMaterial(Material mat)
    {
        GameObject model = GetComponentInChildren<MaterialChange>().gameObject;
        foreach (int index in indexesOfMaterial)
        {
            Material[] clone = model.GetComponent<Renderer>().materials;
            clone[index] = mat;
            model.GetComponent<Renderer>().materials = clone;
        }
    }

    public void UpdateMaterial(Color col)
    {
        GameObject model = GetComponentInChildren<MaterialChange>().gameObject;
        foreach (int index in indexesOfMaterial)
        {
            Material[] clone = model.GetComponent<Renderer>().materials;
            clone[index].color = col;
            model.GetComponent<Renderer>().materials = clone;
        }
    }

    public void EquipCosmetic(CosmeticItem item)
    {
        info.currentSet.UpdateSet(item);
        info.currentSet.SaveSet(info);
        RemoveCosmetics();
        DisplayCosmetics();
    }

    public void RemoveCosmetics()
    {
        foreach (KeyValuePair<CosmeticType, CosmeticItem> kvp in info.currentSet.cosmetics)
        {
            CosmeticItem item = info.currentSet.cosmetics[kvp.Key];
            if (item.referencedObject != null) Destroy(item.referencedObject);
        }
    }

    public void DisplayCosmetics()
    {
        foreach (KeyValuePair<CosmeticType, CosmeticItem> kvp in info.currentSet.cosmetics)
        {
            if (kvp.Value.name.Length > 0)
            {
                CosmeticItem item = info.currentSet.cosmetics[kvp.Key];
                item.referencedObject = Instantiate(kvp.Value.model, gameObject.transform);
                item.referencedObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));
                SetChildrenLayers(this.gameObject);
                item.referencedObject.tag = gameObject.tag;
            }
        }
    }

    public void NetworkDisplayCosmetics(List<String> CurrentSetNames)
    {
        int count = CurrentSetNames.Count;
        for (int i = 0; i < info.cosmetics.Count; i++)
        {
            if (count < 1) break;
            if (!CurrentSetNames.Contains(info.cosmetics[i].name)) continue;

            CosmeticItem item = info.cosmetics[i];
            item.referencedObject = Instantiate(info.cosmetics[i].model, gameObject.transform);
            item.referencedObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));
            SetChildrenLayers(this.gameObject);
            item.referencedObject.tag = gameObject.tag;
            count--;
        }
    }

    private void SetChildrenLayers(GameObject obj)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            child.gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer)); ;
            SetChildrenLayers(child.gameObject);
        }
    }

}

[System.Serializable]
public class CosmeticSet : ISerializationCallbackReceiver
{
    public List<CosmeticType> _keys = new List<CosmeticType>();
    public List<CosmeticItem> _values = new List<CosmeticItem>();
    
    public Dictionary<CosmeticType, CosmeticItem> cosmetics;

    public CosmeticSet()
    {
        cosmetics = new Dictionary<CosmeticType, CosmeticItem>();
        var types = Enum.GetValues(typeof(CosmeticType));
        foreach (CosmeticType type in types)
        {
            cosmetics.Add(type, null);
        }
    }

    public void UpdateSet(CosmeticItem item)
    {
        if (!cosmetics.ContainsKey(item.type)) return;
        
        cosmetics[item.type] = item;
    }

    public void SaveSet(CharacterInfo info)
    {
        CharacterInfo toSave = (CharacterInfo)ProgressionSystem.Instance.Characters[info.characterName];
        toSave.currentSet = this;
    }

    public List<string> NamesOfCosmetics()
    {
        List<string> names = new List<string>();
        foreach (CosmeticItem item in cosmetics.Values)
        {
            if (item.name.Length > 0) names.Add(item.name);
        }
        return names;
    }

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in cosmetics)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        cosmetics = new Dictionary<CosmeticType, CosmeticItem>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            cosmetics.Add(_keys[i], _values[i]);
    }
}


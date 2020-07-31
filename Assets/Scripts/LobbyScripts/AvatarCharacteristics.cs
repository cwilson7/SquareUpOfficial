using System.Collections;
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

    #region Edit Material Functions

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
    #endregion

    #region Cosmetic Display Functions

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
        Dictionary<CosmeticType, CosmeticItem> dict = new Dictionary<CosmeticType, CosmeticItem>();
        foreach (KeyValuePair<CosmeticType, CosmeticItem> kvp in info.currentSet.cosmetics)
        {
            if (kvp.Value.name.Length > 0)
            {
                CosmeticItem item = info.currentSet.cosmetics[kvp.Key];
                dict.Add(kvp.Key, InstantiateCosmetic(item));
            }
        }
        info.currentSet.cosmetics = dict;
    }

    public void NetworkDisplayCosmetics(List<String> CurrentSetNames)
    {
        int count = CurrentSetNames.Count;
        Dictionary<CosmeticType, CosmeticItem> dict = new Dictionary<CosmeticType, CosmeticItem>();
        for (int i = 0; i < info.cosmetics.Count; i++)
        {
            if (count < 1) break;
            if (!CurrentSetNames.Contains(info.cosmetics[i].name)) continue;

            CosmeticItem item = info.cosmetics[i];
            dict.Add(info.cosmetics[i].type, InstantiateCosmetic(item));
            count--;
        }
        info.currentSet.cosmetics = dict;
    }
    #endregion

    #region Helper Functions
    private CosmeticItem InstantiateCosmetic(CosmeticItem item)
    {
        Armature armature = GetComponentInChildren<Armature>();
        item.referencedObject = Instantiate(item.model, gameObject.transform);
        item.referencedObject.transform.SetParent(armature.gameObject.transform);
        item.referencedObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));
        SetChildrenLayers(this.gameObject);
        item.referencedObject.tag = gameObject.tag;
        return item;
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
    #endregion

}

#region Cosmetic Set Class
[System.Serializable]
public class CosmeticSet : ISerializationCallbackReceiver
{
    [HideInInspector] public List<CosmeticType> _keys = new List<CosmeticType>();
    public List<CosmeticItem> currentCosmetics = new List<CosmeticItem>();
    
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
        /* if (!cosmetics.ContainsKey(item.type))
        {
            Debug.Log("No cosmetic by the name of " + item.name + " found.");
            return;
        } */
        
        cosmetics[item.type] = item;
    }

    public void SaveSet(CharacterInfo info)
    {
        CharacterInfo toSave = ProgressionSystem.Instance.Characters[info.characterName];
        toSave.currentSet = this;
    }

    public List<string> NamesOfCosmetics()
    {
        List<string> names = new List<string>();
        foreach (CosmeticItem item in cosmetics.Values)
        {
            if (item != null && item.name.Length > 0) names.Add(item.name);
        }
        return names;
    }

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        currentCosmetics.Clear();

        foreach (var kvp in cosmetics)
        {
            _keys.Add(kvp.Key);
            currentCosmetics.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        cosmetics = new Dictionary<CosmeticType, CosmeticItem>();

        for (int i = 0; i != Math.Min(_keys.Count, currentCosmetics.Count); i++)
            cosmetics.Add(_keys[i], currentCosmetics[i]);
    }
}

#endregion


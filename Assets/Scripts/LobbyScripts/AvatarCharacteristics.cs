using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using CustomUtilities;
using System;
using UnityEngine.SceneManagement;

public class AvatarCharacteristics : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterInfo info;
    public int[] indexesOfMaterial;
    public VideoClip myDemo;
    public GameObject FistModel, defaultFist;

    [Header("Cosmetic Folder Information")]
    [Tooltip("Enter only the name of specified character's cosmetics folder")]
    [SerializeField] private string CosmeticFolder;

    [Tooltip("Enter name of folder and type of cosmetics that that folder houses")]
    [SerializeField] private CosmeticLoader[] folders;

    public GameObject lFist, rFist;

    private void Awake()
    {
        info = ProgressionSystem.CharacterData(info);
        if (info.currentSet.cosmetics.ContainsKey(CosmeticType.Fist) && !info.currentSet.cosmetics[CosmeticType.Fist].IsNull())
        {
            FistModel = info.currentSet.cosmetics[CosmeticType.Fist].model;
        }
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(2))
        {
            SpawnDummyFists();
        }
    }

    //we want to instantiate cosmetics and assign their references

    private void Start()
    {
        //checks if current scene is not the game scene bcz needs to be networked in game
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(2))
        {
            DisplayAllCosmetics();
        }

    }

    public void SpawnDummyFists()
    {
        LFist lLoc = GetComponentInChildren<LFist>();
        RFist rLoc = GetComponentInChildren<RFist>();

        lFist = Instantiate(FistModel, lLoc.gameObject.transform);
        rFist = Instantiate(FistModel, rLoc.gameObject.transform);

        GameObject[] fists = { lFist, rFist };

        if (info.currentSet.cosmetics.ContainsKey(CosmeticType.Fist))
        {
            AssignFistModels(info.currentSet.cosmetics[CosmeticType.Fist]);
        }

        lFist.GetComponent<Fist>().Origin = lLoc.gameObject.transform;
        rFist.GetComponent<Fist>().Origin = rLoc.gameObject.transform;

        lFist.GetComponent<Fist>().InitializeDummy();
        rFist.GetComponent<Fist>().InitializeDummy();

        lFist.GetComponent<Rigidbody>().isKinematic = true;
        rFist.GetComponent<Rigidbody>().isKinematic = true;

        lFist.layer = gameObject.layer;
        rFist.layer = gameObject.layer;
    }

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
        if (lFist != null && rFist != null)
        {
            SetFistMaterial(lFist, mat.color);
            SetFistMaterial(rFist, mat.color);
        }
    }

    public void SetFistMaterial(GameObject fist, Color _color)
    {
        Fist script = fist.GetComponent<Fist>();
        int[] indexes = script.materialIndexesToChange;
        for (int i = 0; i < indexes.Length; i++)
        {
            fist.GetComponent<Renderer>().materials[i].color = _color;
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
        DisplayCosmetic(item);
    }

    public void DisplayCosmetic(CosmeticItem item)
    {
        if (item.type == CosmeticType.Fist)
        {
            FistModel = info.currentSet.cosmetics[CosmeticType.Fist].model;
            SpawnDummyFists();
        }
        else InstantiateCosmetic(item);
    }

    public void DisplayAllCosmetics()
    {
        Dictionary<CosmeticType, CosmeticItem> dict = new Dictionary<CosmeticType, CosmeticItem>();
        foreach (KeyValuePair<CosmeticType, CosmeticItem> kvp in info.currentSet.cosmetics)
        {
            if (!kvp.Value.IsNull())//name.Length > 0)
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

    public void AssignFistModels(CosmeticItem item)
    {
        if (item.type == CosmeticType.Fist)
        {
            item.referencedObjects = new GameObject[] { lFist, rFist };
            for (int i = 0; i < item.referencedObjects.Length; i++)
            {
                item.referencedObjects[i].layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));
                item.referencedObjects[i].tag = gameObject.tag;
            }
        }
    }
    private CosmeticItem InstantiateCosmetic(CosmeticItem item)
    {
        if (item.type != CosmeticType.Fist)
        {
            Armature armature = GetComponentInChildren<Armature>();
            item.referencedObject = Instantiate(item.model, gameObject.transform);
            item.referencedObject.transform.SetParent(armature.gameObject.transform);
            item.referencedObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer));
            item.referencedObject.tag = gameObject.tag;
        }

        SetChildrenLayers(this.gameObject);

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
    }

    public void UpdateSet(CosmeticItem item)
    {
        if (cosmetics.ContainsKey(item.type))
        {
            if (!cosmetics[item.type].IsNull())
            {
                if (item.type != CosmeticType.Fist) GameObject.Destroy(cosmetics[item.type].referencedObject);
                else
                {
                    for (int i = 0; i < cosmetics[item.type].referencedObjects.Length; i++)
                    {
                        GameObject.Destroy(cosmetics[item.type].referencedObjects[i]);
                    }
                }
            }
        }
        if (!cosmetics.ContainsKey(item.type))
        {
            cosmetics.Add(item.type, item);
        }
        else cosmetics[item.type] = item;
    }

    public void SaveSet(CharacterInfo info)
    {
        List<CharacterInfo> newList = new List<CharacterInfo>();
        foreach (CharacterInfo ci in ProgressionSystem.playerData.characters)
        {
            if (ci.characterName == info.characterName)
            {
                newList.Add(info);
            }
            else newList.Add(ci);
        }
        ProgressionSystem.playerData.Characters = newList;
        ProgressionSystem.SaveData();
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

    public void SetItemsActive(bool isActive)
    {
        var types = Enum.GetValues(typeof(CosmeticType));
        foreach (CosmeticType type in types)
        {
            if (cosmetics.ContainsKey(type))
            {
                if (type != CosmeticType.Fist) cosmetics[type].referencedObject.SetActive(isActive);
                else
                {
                    for (int i = 0; i < cosmetics[type].referencedObjects.Length; i++)
                    {
                        cosmetics[type].referencedObjects[i].SetActive(isActive);
                    }
                }
            }
        }
    }
}

#endregion


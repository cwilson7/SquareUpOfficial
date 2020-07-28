using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using CustomUtilities;
using System;
using UnityEngine.Events;

public class AvatarCharacteristics : MonoBehaviour
{
    public int[] indexesOfMaterial;
    public List<Level> MyLevels;
    public VideoClip myDemo;
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
}

public class CosmeticSet
{
    //we have some set of cosmetic items
    //display these items on character and save in progression system
}

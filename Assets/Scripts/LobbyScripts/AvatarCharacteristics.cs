using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using CustomUtilities;
using System;

public class AvatarCharacteristics : MonoBehaviour
{
    public int[] indexesOfMaterial;
    public List<Level> MyLevels;
    public VideoClip myDemo;
    [Tooltip("Enter only the name of specified character's cosmetics folder")]
    [SerializeField] private string CosmeticFolder;

    [SerializeField] private Tuple<string, CosmeticType>[] folders;

    public List<CosmeticItem> LoadCosmetics()
    {
        List<CosmeticItem> ReturnList = new List<CosmeticItem>();
        
        List <GameObject> fistCosmetics = new List<GameObject>(), headCosmetics = new List<GameObject>(), bodyCosmetics = new List<GameObject>(), accCosmetics = new List<GameObject>();
        string fistPath = "Fists", headPath = "Head", bodyPath = "Body", accPath = "Acc";
        SetPath(fistPath);
        SetPath(headPath);
        SetPath(bodyPath);
        SetPath(accPath);
        
        return ReturnList;
    }

    private void SetPath(string path)
    {
        path = "PhotonPrefabs/Cosmetics/" + CosmeticFolder + path;
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

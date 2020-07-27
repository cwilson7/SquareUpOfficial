using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticOptionsHandler : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup cosmeticsGrid;
    
    [Tooltip("Enter only the name of the folder with desired cosmetic items")]
    [SerializeField] private string CosmeticFolder;
    private string CosmeticFolderPath;
    
    // Start is called before the first frame update
    void Start()
    {
        CosmeticFolderPath = "PhotonPrefabs/Cosmetics/" + CosmeticFolder;
    }


    public void SliderFunction()
    {

    }
}

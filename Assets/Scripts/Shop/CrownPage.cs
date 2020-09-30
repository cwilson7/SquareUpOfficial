using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CustomUtilities;
using UnityEngine.Events;

public class CrownPage : MonoBehaviour
{
    GridLayoutGroup layoutGroup;
    public int options_per_row, options_per_column;
    [SerializeField] TMP_Text unlockedTracker;
    [SerializeField] GameObject crownOptionPrefab;
    [SerializeField] Transform crownDisplayLocation;
    public GameObject currentlyDisplayedCrown;

    private void Start()
    {
        layoutGroup = GetComponentInChildren<GridLayoutGroup>();
        LoadGrid();
    }

    void LoadGrid()
    {
        RectTransform gridRect = layoutGroup.gameObject.GetComponent<RectTransform>();

        layoutGroup.cellSize = new Vector2(gridRect.rect.width / options_per_row, gridRect.rect.height / options_per_column);
        layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layoutGroup.constraintCount = options_per_row;

        string filePath = ProgressionSystem.playerData.crownPath;
        CrownData[] dataArray = ProgressionSystem.playerData.crownDataArray;
        List<string> unlockedCrownNames = new List<string>();
        List<string> lockedCrownNames = new List<string>();
        int numUnlocked = 0, total = dataArray.Length;
        for (int i = 0; i < total; i++)
        {
            if (dataArray[i].status == Status.Unlocked)
            {
                unlockedCrownNames.Add(dataArray[i].name);
                numUnlocked++;
            }
            else lockedCrownNames.Add(dataArray[i].name);
        }
        unlockedTracker.text = "You have unlocked " + numUnlocked + '/' + total + " crowns";

        //this only shows unlocked crowns
        foreach (string crownName in unlockedCrownNames)
        {
            GameObject option = Instantiate(crownOptionPrefab, layoutGroup.transform);
            CrownOption optionScript = option.GetComponent<CrownOption>();
            optionScript.crownDisplayLocation = this.crownDisplayLocation;
            optionScript.SetOption(Resources.Load<GameObject>(filePath+crownName));           
        }
        
        //each option will have the item name
        //maybe two grids
        //unlocked and locked
        //or just show unlocked
    }

    public void OpenPanel(GameObject pnl)
    {
        if (pnl == this.gameObject && currentlyDisplayedCrown != null) currentlyDisplayedCrown.SetActive(true);
        pnl.SetActive(true);
    }

    public void ClosePanel(GameObject pnl)
    {
        if (pnl == this.gameObject) currentlyDisplayedCrown.SetActive(false); 
        pnl.SetActive(false);
    }
}

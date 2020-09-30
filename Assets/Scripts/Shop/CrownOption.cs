using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrownOption : MonoBehaviour
{
    CrownPage parentPage;
    GameObject myCrown;
    string myCrownName;
    Button thisButton;
    [HideInInspector] public Transform crownDisplayLocation;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        parentPage = GetComponentInParent<CrownPage>();
        thisButton.onClick.AddListener(SelectOption);
    }

    public void SetOption(GameObject crownPrefab)
    {
        GetComponentInChildren<TMP_Text>().text = crownPrefab.name;

        myCrownName = crownPrefab.name;
        myCrown = Instantiate(crownPrefab, crownDisplayLocation);
        myCrown.transform.localPosition = Vector3.zero;
        myCrown.transform.forward = Camera.main.transform.position - myCrown.transform.position; 
        if (crownPrefab.name == ProgressionSystem.playerData.myCrownName)
        {
            myCrown.SetActive(true);
            parentPage.currentlyDisplayedCrown = myCrown;
        }
        else myCrown.SetActive(false);
    }

    void SelectOption()
    {
        CrownData[] dataArray = ProgressionSystem.playerData.crownDataArray;
        for (int i = 0; i < dataArray.Length; i++)
        {
            Debug.Log("name of array item: " + dataArray[i].name + " and my crown name is: " + myCrown.name);
            if (dataArray[i].name == myCrownName)
            {
                Debug.Log("Found a match!!");
                ProgressionSystem.playerData.myCrownName = myCrownName;
                ProgressionSystem.SaveData();
                break;
            }
        }

        //display object somewhere
        parentPage.currentlyDisplayedCrown.SetActive(false);
        myCrown.SetActive(true);
        parentPage.currentlyDisplayedCrown = myCrown;
    }
}

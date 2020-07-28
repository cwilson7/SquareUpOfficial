using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
    public static event Action<ProgressionSystem> SaveGame;
    
    public static ShopController Instance;
    
    [SerializeField] private GameObject ShopPanelPrefab;

    public GameObject mainPanel;

    public List<GameObject> characterPanels;

    void Start()
    {
        Instance = this;
        List<GameObject> charAvatars = new List<GameObject>();
        characterPanels = new List<GameObject>();
        Utils.PopulateList<GameObject>(charAvatars, "PhotonPrefabs/CharacterAvatars");
        for (int j = 0; j < charAvatars.Count; j++)
        {
            characterPanels.Add(GenerateShopPanel(charAvatars[j]));
        }
        mainPanel.GetComponent<MainPanel>().InitializeCharacterButtons();
    }

    GameObject GenerateShopPanel(GameObject _character)
    {
        GameObject panel = Instantiate(ShopPanelPrefab, GameObject.Find("Canvas").transform);
        ShopPanel shopPanel = panel.GetComponent<ShopPanel>();
        shopPanel.Character = _character;
        shopPanel.Setup();
        shopPanel.DontDisplay();
        return panel;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
        SaveGame?.Invoke(ProgressionSystem.Instance);
        //Destroy(ProgressionSystem.Instance.gameObject);
        Destroy(AudioManager.AM.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;
using TMPro;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private GameObject CharacterButtonPrefab;
    
    Button[] CharacterButtons;
    
    public void InitializeCharacterButtons()
    {
        /*
        List<GameObject> charAvatars = new List<GameObject>();
        Utils.PopulateList<GameObject>(charAvatars, "PhotonPrefabs/CharacterAvatars");
        CharacterButtons = new Button[charAvatars.Count];
        for (int j = 0; j < charAvatars.Count; j++)
        {
            CharacterButtons[j] = (GenerateCharacterButton(charAvatars[j]));
        }
        */
    }

    Button GenerateCharacterButton(GameObject _char)
    {
        GameObject bGO = Instantiate(CharacterButtonPrefab, GetComponentInChildren<HorizontalLayoutGroup>().gameObject.transform);
        Button _button = bGO.GetComponent<Button>();
        TMP_Text label = bGO.GetComponentInChildren<TMP_Text>();
        label.text = _char.GetComponent<AvatarCharacteristics>().info.characterName;// .name;
        SelectCharacterPanelButton script = bGO.GetComponent<SelectCharacterPanelButton>();
        script.character = _char;

        return _button;
    }
}

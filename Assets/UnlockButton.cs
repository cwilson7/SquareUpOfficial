using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtilities;

public class UnlockButton : MonoBehaviour
{
    public static event Action<ProgressionSystem> ProgressionSystemChange;
    private string associatedCharacterName;
    private ShopPanel parentPanel;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(UnlockCharacter);
        parentPanel = Utils.FindParentWithClass<ShopPanel>(transform);
        associatedCharacterName = parentPanel.charInfo.characterName;
    }

    public void UnlockCharacter()
    {
        ProgressionSystem ps = ProgressionSystem.Instance;

        if (!ps.Characters.ContainsKey(associatedCharacterName))
        {
            Debug.Log("Invalid Character Name");
            return;
        }

        CharacterInfo info = (CharacterInfo)ps.Characters[associatedCharacterName];
        info.status = Status.Unlocked;

        parentPanel.SetPanelLockedInfo();

        ProgressionSystemChange?.Invoke(ps);
    }
}

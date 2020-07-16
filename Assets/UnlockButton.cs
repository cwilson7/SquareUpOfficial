using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockButton : MonoBehaviour
{
    public static event Action<ProgressionSystem> ProgressionSystemChange;
    public string associatedCharacterName;

    public void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UnlockCharacter);
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

        ProgressionSystemChange?.Invoke(ps);
    }
}

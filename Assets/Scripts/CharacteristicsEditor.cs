using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(AvatarCharacteristics))]
public class CharacteristicsEditor : Editor
{
    /*
    CosmeticSet set;

    public override void OnInspectorGUI()
    {
        AvatarCharacteristics characteristics = (AvatarCharacteristics)target;
        set = characteristics.cosmeticSet;
        if (set != null)
        {
            foreach (KeyValuePair<CosmeticType, CosmeticItem> kvp in set.cosmetics)
            {
                characteristics.cosmeticSet = (CosmeticSet)EditorGUILayout.ObjectField(kvp.Key.ToString(), kvp.Value.model, typeof(GameObject), true);
            }
        }
        base.OnInspectorGUI();
    }
    */
}


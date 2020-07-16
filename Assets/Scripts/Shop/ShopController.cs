using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static event Action<ProgressionSystem> ProgressionSystemChange;

    [SerializeField] private Button[] CharacterUnlockButtons;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

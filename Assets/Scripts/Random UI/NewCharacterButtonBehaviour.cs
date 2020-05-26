using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Button>().onClick.AddListener(SelectDifferentCharacter);
    }

    // Update is called once per frame
    private void SelectDifferentCharacter()
    {

    }
}

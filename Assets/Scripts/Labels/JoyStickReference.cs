using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickReference : MonoBehaviour
{
    public static JoyStickReference joyStick;
    
    // Start is called before the first frame update
    void Awake()
    {
        joyStick = this;
    }
}

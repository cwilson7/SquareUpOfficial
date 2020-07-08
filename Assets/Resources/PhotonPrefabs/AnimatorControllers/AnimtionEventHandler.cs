using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimtionEventHandler : MonoBehaviour
{
    Controller parentController;
    // Start is called before the first frame update
    public void InitializeEventHandler(Controller pc)
    {
        parentController = pc;
    } 

    // Update is called once per frame
    public void FlinchStart()
    {
        parentController.blockInput = true;
    }
    public void FlinchEnd()
    {
        parentController.blockInput = false;
    }
    public void MeleeStart()
    {
        parentController.speed /= 4;
    }
    public void MeleeEnd()
    {
        parentController.speed *= 4;
    }
}

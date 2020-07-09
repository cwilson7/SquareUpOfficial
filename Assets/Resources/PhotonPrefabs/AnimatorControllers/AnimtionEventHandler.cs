using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimtionEventHandler : MonoBehaviour
{
    Controller parentController;
    public float meleeSlow;
    // Start is called before the first frame update
    public void InitializeEventHandler(Controller pc)
    {
        parentController = pc;
        meleeSlow = 0.35f;
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
        StartCoroutine(MeleeSlow(meleeSlow));
    }
    private IEnumerator MeleeSlow(float delay)
    {
        yield return new WaitForSeconds(delay);
        parentController.speed *= 4;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrBusinessController : Controller
{
    private SkinnedMeshRenderer paper;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        //paper = GetComponentInChildren<Special>().gameObject.GetComponent<SkinnedMeshRenderer>();
        audioKey = "MrBusiness";
        audioHandler.InitializeAudio(audioKey);
    }

    public override void SpecialAbility()
    {
        anim.SetTrigger("Special");

        StartCoroutine(PaperRenderer());
    }

    IEnumerator PaperRenderer()
    {
        yield return new WaitForSeconds(.1f);
        //paper.enabled = true;
        yield return new WaitForSeconds(1f);
        //paper.enabled = false;
    }
}

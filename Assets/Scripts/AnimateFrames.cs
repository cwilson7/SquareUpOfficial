using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.Utility;

public class AnimateFrames : MonoBehaviour
{
    [SerializeField] string folderPath;
    [SerializeField] float framesPerSecond;
    [SerializeField] Image display;
    [SerializeField] Dictionary<Multikill, int> specialStopFrames = new Dictionary<Multikill, int>();
    [Header("Should only be as many as there are multkills.")]
    [SerializeField] int[] indexesToStop;
    float index = 0f, finalIndex, t = 0f;
    Sprite[] frames;
    public float startTime, fadeDuration = 1f;
    bool fading = false;
    
    // Start is called before the first frame update
    void Start()
    {
        frames = Resources.LoadAll<Sprite>(folderPath);
        foreach (Multikill key in Enum.GetValues(typeof(Multikill)))
        {
            if (key > Multikill.Quadra) continue;
            specialStopFrames.Add(key, indexesToStop[(int)key]);
        }
    }

    private void FixedUpdate()
    {
        if (fading)
        {
            t += Time.deltaTime / fadeDuration;
            Color _col = display.color;
            float a = _col.a;
            a = Mathf.Lerp(a, 0, t);
            display.color = new Color(_col.r, _col.g, _col.b, a);
            if (a <= 0)
            {
                t = 0f;
                fading = false;
                display.enabled = false;
            }
        }
    }

    public void SetAnimationFrames(Multikill multikill)
    {
        finalIndex = specialStopFrames[multikill];
        display.enabled = true;
        if (fading)
        {
            fading = false;
            t = 0f;
        }
        Color _col = display.color;
        display.color = new Color(_col.r, _col.g, _col.b, 1);
    }

    // Update is called once per frame
    public void Animate()
    {
        index = ((Time.time - startTime) * framesPerSecond) % frames.Length;
        if (index <= finalIndex) display.sprite = frames[(int)index];
        else startTime += Time.deltaTime;
    }

    public void EndAnimation()
    {
        index = 0f;
        fading = true;
    }
}

// i want to grab the full animation
// segment into "multikills"
// based on which multikill play from anim[multikill-1] to anim[multkill]
// playanim(start, end)
// iterate from start to end then stay on end till its over
// if i call again during that anim, reach final index, then play from anim[new - 1] to anim[new]
// when is the func being called?
// in update, when timer is up set index back to 0


public enum Multikill
{
    Single,
    Double,
    Triple,
    Quadra,
    Penta
} 

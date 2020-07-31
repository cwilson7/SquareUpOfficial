using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AM;
    
    private AudioSource currentTheme;
    public int lastBuildIndex;
    [SerializeField] private AudioClip mainTheme;

    private void Awake()
    {
        AudioManager.AM = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        currentTheme = GetComponent<AudioSource>();
        lastBuildIndex = 0;
        SceneManager.sceneLoaded += SwitchThemeScene;
        Cube.CubeRotated += SwitchThemeLevel;
    }

    void SwitchThemeLevel(Level level)
    {
        SwitchTrack(level.theme);
    }

    void SwitchThemeScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        
        switch(scene.buildIndex)
        {
            //main scene
            case 0 :
                //if we are already playing the theme, return.
                //problem is this gets deleted or something
                if (lastBuildIndex != 0)
                {
                    SwitchTrack(mainTheme);
                    lastBuildIndex = 0;
                }
                break;

            //lobby scene
            case 1 :
                
                break;

            //game scene
            case 2:
                if (AudioManager.AM != this) return;
                StartCoroutine(WaitForCube());
                if (Cube.cb == null) return;
                Level level = Cube.cb.CurrentFace;
                if (level == null) return;
                SwitchTrack(level.theme);
                lastBuildIndex = 2;
                    break;
        }
    }

    IEnumerator WaitForCube()
    {
        yield return new WaitForSeconds(1f);
        if (Cube.cb == null) StartCoroutine(WaitForCube());
        else SwitchTrack(Cube.cb.CurrentFace.theme);
    }

    void SwitchTrack(AudioClip track)
    {
        if (AM == null) return;
        Debug.Log("switch track getting called");
        currentTheme.Stop();
        currentTheme.clip = track;
        currentTheme.Play();
    }

   // private String

}

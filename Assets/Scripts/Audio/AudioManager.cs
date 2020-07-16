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
    [SerializeField] private AudioClip mainTheme;

    void Start()
    {
        if (AudioManager.AM != null)
        {
            Destroy(this);
            Destroy(this.gameObject);
            return;
        }
        AudioManager.AM = this;
        currentTheme = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += SwitchThemeScene;
        Cube.CubeRotated += SwitchThemeLevel;
        DontDestroyOnLoad(this.gameObject);
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
                SwitchTrack(mainTheme);
                break;

            //lobby scene
            case 1 :
                
                break;

            //game scene
            case 2:
                if (AudioManager.AM == null) return;
                StartCoroutine(WaitForCube());
                if (Cube.cb == null) return;
                Level level = Cube.cb.CurrentFace;
                if (level == null) return;
                SwitchTrack(level.theme);   
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
        currentTheme.Stop();
        currentTheme.clip = track;
        currentTheme.Play();
    }

   // private String

}

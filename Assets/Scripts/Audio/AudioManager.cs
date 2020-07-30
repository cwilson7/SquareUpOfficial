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
    bool initalized = false;
    public int lastBuildIndex;
    [SerializeField] private AudioClip mainTheme;


    private void Awake()
    {
        Debug.Log(AudioManager.AM != this);
        if (AM == null)
        {
            AM = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (AM != this)
        {
            Debug.Log("destroy stuff getting called");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("new adio manajer");
        Debug.Log("more trash garbage gettting claaed");
        initalized = true;
        currentTheme = GetComponent<AudioSource>();
        currentTheme.enabled = true;
        lastBuildIndex = 1000;
        SceneManager.sceneLoaded += SwitchThemeScene;
        Cube.CubeRotated += SwitchThemeLevel;
        SwitchThemeScene(SceneManager.GetActiveScene(), LoadSceneMode.Additive);
        Debug.Log(AudioManager.AM == null);
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

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
        if (AudioManager.AM == null)
        {
            AudioManager.AM = this;
        }
        else
        {
            if (AudioManager.AM != this)
            {
                Destroy(this.gameObject);
            }
        }
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
            //game scene
            case 0:
                if (lastBuildIndex != 0)
                {
                    SwitchTrack(mainTheme);
                    lastBuildIndex = 0;
                }
                break;
            case 2:
                lastBuildIndex = 2;
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
}

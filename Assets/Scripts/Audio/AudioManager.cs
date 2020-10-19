using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AM;
    
    public AudioSource currentTheme, audio2;
    public int lastBuildIndex;
    [SerializeField] private AudioClip mainTheme;
    AudioClip[] killSounds;

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
        killSounds = Resources.LoadAll<AudioClip>("Audio/KillSounds");
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

    public void KillSignifier(Multikill multikill)
    {
        AudioClip s = Array.Find<AudioClip>(killSounds, AudioClip => AudioClip.name == multikill.ToString());
        if (s == null)
        {
            Debug.Log("Bad clip name or key");
            return;
        }
        audio2.clip = s;
        audio2.Play();
    }
}

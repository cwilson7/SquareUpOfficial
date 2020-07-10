using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;
using System;

public class AudioHandler : MonoBehaviour
{
    public Sound[] allSounds;
    public AudioSource audioSource;


    public void InitializeAudio(string key)
    {
        //audioSource = GetComponent<AudioSource>();
        //Utils.PopulateList<Sound>(allSounds, "PhotonPrefabs/AudioClips");
        foreach (Sound s in allSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.spatialBlend = 1;
            s.source.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    public void Play(string key, string name)
    {
        Sound s = Array.Find(allSounds, Sound => Sound.name == key+name);
        if (s == null)
        {
            Debug.Log("Bad clip name or key");
            return;
        }
        s.source.Play();
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PersistentManager<SoundManager>
{

    [Header("Mixer")]
    public AudioMixer globalMixer;

    public void SetGlobalVolume(float volume)
    {
        globalMixer.SetFloat("GlobalVolume", Mathf.Log10(volume) * 20); // converts [0-1] to decibels
    }


    public void SetMusicVolume(float volume)
    {
        globalMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // converts [0-1] to decibels
    }

    public void SetSFXVolume(float volume)
    {
        globalMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}

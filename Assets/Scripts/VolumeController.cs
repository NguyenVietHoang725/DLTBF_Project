using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource musicSource;
    public AudioSource[] sfxSources;


    void Start()
    {
        // Retrieve the music source from the MusicManager
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicSource = musicManager.GetMusicSource();
            musicSlider.value = musicSource.volume;
        }

        sfxSlider.value = GetSFXVolume();

        // Add listeners to sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    void SetSFXVolume(float volume)
    {
        foreach (var sfx in sfxSources)
        {
            sfx.volume = volume;
        }
    }

    float GetSFXVolume()
    {
        if (sfxSources.Length > 0)
        {
            return sfxSources[0].volume; // Return the volume of the first SFX source
        }
        return 1.0f; // Default volume if no SFX sources are assigned
    }
}

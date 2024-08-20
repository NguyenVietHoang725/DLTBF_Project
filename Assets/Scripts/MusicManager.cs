using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;
    public AudioSource musicSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate MusicManager
        }
    }

    public AudioSource GetMusicSource()
    {
        return musicSource;
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play(); // Resume playing the music if it was stopped
        }
    }

    public void PlayWinnerSound(AudioClip winnerSound)
    {
        musicSource.Stop();
        AudioSource.PlayClipAtPoint(winnerSound, Vector3.zero);

        Invoke("ResumeBackgroundMusic", winnerSound.length);
    }

    private void ResumeBackgroundMusic()
    {
        MusicManager.instance.ResumeMusic();
    }
}
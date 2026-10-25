using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerScript : MonoBehaviour
{
    [SerializeField] List<MusicObject> musicObjects;
    [SerializeField] float defaultFadeTime = 1;

    [SerializeField] List<AudioSource> AudioSources;
    List<bool> AudioSourceIsPrimary = new List<bool>();

    public static MusicPlayerScript instance;

    float InitialFadeTime;
    float currentFadeTime;
    int audioSourceToFadeIn;
    float volumeTarget = 1.0f;
    List<int> audioSourcesToFadeOut = new List<int>();

    void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < AudioSources.Count; i++)
        {
            AudioSourceIsPrimary.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        fadeUpdate();
    }

    public void setVolume(float volume)
    {
        volumeTarget = volume;
    }
    public void fade(string songName)
    {
        fade(songName, defaultFadeTime);
    }
    public void fade(string songName, float time)
    {
        MusicObject songToPlay = findSongByName(songName);

        audioSourceToFadeIn = findAvalibleAudioSource();

        AudioSources[audioSourceToFadeIn].clip = songToPlay.songFile;

        currentFadeTime = time;
        InitialFadeTime = time;

        audioSourcesToFadeOut = findInUseAudioSources();

        AudioSources[audioSourceToFadeIn].Play();

        AudioSourceIsPrimary[audioSourceToFadeIn] = true;
    }

    private void fadeUpdate()
    {
        if (currentFadeTime > 0.0f)
        {
            currentFadeTime -= Time.deltaTime;

            currentFadeTime = Mathf.Clamp(currentFadeTime, 0.0f, InitialFadeTime);

            for (int i = 0; i < audioSourcesToFadeOut.Count; i++)
            {
                AudioSources[audioSourcesToFadeOut[i]].volume = currentFadeTime / InitialFadeTime * volumeTarget;

                if (currentFadeTime == 0.0f)
                {
                    AudioSourceIsPrimary[i] = false;
                }
            }
            AudioSources[audioSourceToFadeIn].volume = (1.0f - (currentFadeTime / InitialFadeTime)) * volumeTarget;
        }
    }

    private MusicObject findSongByName(string songName)
    {
        for (int i = 0; i < musicObjects.Count; i++)
        {
            if (musicObjects[i].songName == songName)
            {
                return musicObjects[i];
            }
        }

        Debug.LogWarning("Incorrect song name or song not found.");
        return musicObjects[0];
    }

    private int findAvalibleAudioSource()
    {
        for (int i = 0; i < AudioSources.Count; i++)
        {
            if (!AudioSourceIsPrimary[i])
            {
                return i;
            }
        }

        Debug.LogWarning("Open Audio Source not found for music transition.");
        return 0;
    }

    private List<int> findInUseAudioSources()
    {
        List<int> foundSources = new List<int>();

        for (int i = 0; i < AudioSources.Count; i++)
        {
            if (AudioSourceIsPrimary[i])
            {
                foundSources.Add(i);
            }
        }

        return foundSources;
    }
}

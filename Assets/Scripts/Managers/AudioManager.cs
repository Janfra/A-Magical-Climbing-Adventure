using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource musicSource;

    [System.Serializable]
    public class Audio
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public AudioClip clip;
    }

    [System.Serializable]
    public class OnLoadMusic : Audio
    {
        [SerializeField]
        public int buildIndexToPlayOn;
    }

    [System.Serializable]
    public class SFXAudio : Audio
    {
        [SerializeField]
        public AudioSource source;
    }

    [SerializeField]
    private List<OnLoadMusic> MusicOnLoadToPlay;
    [SerializeField]
    private List<Audio> MusicToLoad;
    [SerializeField]
    private List<SFXAudio> SFXToLoad;
    private Dictionary<string, Audio> audioDictionary = new();
    private Dictionary<int, Audio> musicDictionary = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        CreateDictionary();
        PlayMusic("TitleScreen");
    }

    public void PlayMusic(string _name)
    {
        if (audioDictionary.ContainsKey(_name))
        {
            musicSource.clip = audioDictionary[_name].clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogError($"Music requested not found, name given: {_name}");
        }
    }

    public void PlayAudio(string _name)
    {
        if (audioDictionary.ContainsKey(_name))
        {
            if(audioDictionary[_name] is SFXAudio)
            {
                SFXAudio audio = (SFXAudio)audioDictionary[_name];
                audio.source.PlayOneShot(audio.clip);
            }
            else
            {
                Debug.LogError($"SFX requested is not a SFX, name given: {_name}");
            }
        }
        else
        {
            Debug.LogError($"Audio requested not found, name given: {_name}");
        }
    }

    public void PlayMusicByIndex(int _index)
    {
        if (musicDictionary.ContainsKey(_index))
        {
            musicSource.clip = musicDictionary[_index].clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music requested on load not found, index given: {_index}");
        }
    }

    public bool IsAudioPlaying(string _name)
    {
        if (audioDictionary.ContainsKey(_name))
        {
            if (audioDictionary[_name] is SFXAudio)
            {
                SFXAudio audio = (SFXAudio)audioDictionary[_name];
                return audio.source.isPlaying;
            }
            else
            {
                Debug.LogError($"SFX requested is not a SFX, name given: {_name}");
            }
        }
        else
        {
            Debug.LogError($"Audio requested not found, name given: {_name}");
        }
        return false;
    }

    public void StopAudio(string _name)
    {
        SFXAudio audio = GetSFXAudio(_name);
        if (audio != null)
        {
            audio.source.Stop();
        }
    }

    private SFXAudio GetSFXAudio(string _name)
    {
        if (audioDictionary.ContainsKey(_name))
        {
            if (audioDictionary[_name] is SFXAudio)
            {
                return (SFXAudio)audioDictionary[_name];
            }
            else
            {
                Debug.LogError($"SFX requested is not a SFX, name given: {_name}");
            }
        }
        else
        {
            Debug.LogError($"Audio requested not found, name given: {_name}");
        }
        return null;
    }

    public void ChangeMusicVolume(float _volume)
    {
        musicSource.volume = Mathf.Clamp01(_volume);
    }

    private void CreateDictionary()
    {
        int indexCount = 0;
        foreach(Audio audio in MusicToLoad)
        {
            if(!audioDictionary.ContainsKey(audio.name))
            {
                audioDictionary.Add(audio.name, audio);
            }
            else
            {
                Debug.LogError($"Music name given twice! Name: {audio.name} at index: {indexCount}");
            }
            indexCount++;
        }

        indexCount = 0;
        foreach(SFXAudio audio in SFXToLoad)
        {
            if (!audioDictionary.ContainsKey(audio.name))
            {
                AudioSource source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                audio.source = source;
                source.clip = audio.clip;
                audioDictionary.Add(audio.name, audio);
            }
            else
            {
                Debug.LogError($"SFX name given twice! Name: {audio.name} at index: {indexCount}");
            }
            indexCount++;
        }

        indexCount = 0;
        foreach (OnLoadMusic music in MusicOnLoadToPlay)
        {
            if (!musicDictionary.ContainsKey(music.buildIndexToPlayOn))
            {
                musicDictionary.Add(music.buildIndexToPlayOn, music);
            }
            else
            {
                Debug.LogError($"Music to play on load index given twice! Name: {music.name} at index: {indexCount}");
            }

            if (!audioDictionary.ContainsKey(music.name))
            {
                audioDictionary.Add(music.name, music);
            }
            else
            {
                Debug.LogWarning($"Music name given twice, music to load is also added to audios, keep on one place! Name: {music.name} at index: {indexCount}");
            }
            indexCount++;
        }
    }

    public static void PlayOnButtonPressAudio()
    {
        if(Instance != null)
        {
            Instance.PlayAudio("MenuSelect");
        }
    }
}

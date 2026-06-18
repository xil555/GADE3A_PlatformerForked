using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip gemSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip healthPickupSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip checkpointSound;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Music")]
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float footstepVolume = 0.7f;

    private readonly HashMapADT<string, AudioClip> sfxMap = new HashMapADT<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
        RegisterSounds();

        if (playMusicOnStart)
        {
            PlayBackgroundMusic();
        }
    }

    private void SetupAudioSources()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
        }

        footstepSource.playOnAwake = false;
        footstepSource.loop = true;
        footstepSource.clip = footstepSound;
        footstepSource.volume = footstepVolume;

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
    }

    private void RegisterSounds()
    {
        AddSound("Gem", gemSound);
        AddSound("Damage", damageSound);
        AddSound("Death", deathSound);
        AddSound("HealthPickup", healthPickupSound);
        AddSound("Footstep", footstepSound);
        AddSound("Checkpoint", checkpointSound);
        AddSound("BackgroundMusic", backgroundMusic);
    }

    public void AddSound(string key, AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxMap.Add(key, clip);
    }

    public void PlaySound(string key)
    {
        AudioClip clip = sfxMap.Lookup(key);

        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetFootstepsPlaying(bool playing)
    {
        if (footstepSource == null || footstepSound == null)
        {
            return;
        }

        footstepSource.clip = footstepSound;

        if (playing)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else if (footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void RemoveSound(string key)
    {
        sfxMap.Remove(key);
    }

    public void PrintSounds()
    {
        sfxMap.Print();
    }
}

using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] private AudioMixer _audioMixer;

    public AudioMixerGroup BgmGroup;
    public AudioMixerGroup SfxGroup;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private AudioClip _uiButtonSoundClip;
    private AudioClip _itemPickUpSoundClip;
    private AudioClip _backSoundClip;
    private AudioClip _portalSoundClip;

    public float MasterVolume { get; private set; } = 1f;
    public float BGMVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;


    public AudioClip BackSoundClip {
        get => _backSoundClip;
        set {
            if (value == null) return;
            else if(_backSoundClip == value) return;
            _backSoundClip = value;
            PlayBackgroundSound(_backSoundClip);
        }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else Destroy(gameObject);

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();

        _bgmSource.outputAudioMixerGroup = BgmGroup;
        _sfxSource.outputAudioMixerGroup = SfxGroup;

        _bgmSource.loop = true;
        _sfxSource.loop = false;
    }

    private void OnDestroy() {
        if(Instance == this) Instance = null;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Start() {
        AudioClip mainBackSound = Resources.Load<AudioClip>("Audio/Main");
        _uiButtonSoundClip = Resources.Load<AudioClip>("Audio/Button_Click");
        _itemPickUpSoundClip = Resources.Load<AudioClip>("Audio/Item_Pickup");
        _portalSoundClip = Resources.Load<AudioClip>("Audio/Portal");
        BackSoundClip = mainBackSound;
    }

    private void OnSceneChanged(Scene prev, Scene next) {
        switch(next.buildIndex) {
            case (int)Scenes.BuildNumber.Main:
                BackSoundClip = Resources.Load<AudioClip>("Audio/Main");
                break;
            case (int)Scenes.BuildNumber.Town:
                BackSoundClip = Resources.Load<AudioClip>("Audio/Town");
                break;
            case (int)Scenes.BuildNumber.Dungeon1:
                BackSoundClip = Resources.Load<AudioClip>("Audio/Dungeon1");
                break;
            case (int)Scenes.BuildNumber.Dungeon2:
                BackSoundClip = Resources.Load<AudioClip>("Audio/Dungeon2");
                break;
            case (int)Scenes.BuildNumber.Dungeon3:
                BackSoundClip = Resources.Load<AudioClip>("Audio/Dungeon3");
                break;
            default:
                break;
        }
    }

    private void PlayBackgroundSound(AudioClip sound) {
        _bgmSource.clip = sound;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void PlayEffectSound(AudioClip sound) {
        if (sound == null) return;

        _sfxSource.pitch = Random.Range(0.98f, 1.02f);
        _sfxSource.PlayOneShot(sound, 0.7f);
    }

    public void MuteBGM() {
        _audioMixer.SetFloat("BGM", -80);
    }

    public void UnmuteBGM() {
        _audioMixer.SetFloat("BGM", BGMVolume);
    }

    public void PlayButtonSound() {
        _sfxSource.PlayOneShot(_uiButtonSoundClip);
    }

    public void PlayItemPickUpSound() {
        _sfxSource.PlayOneShot(_itemPickUpSoundClip);
    }

    public void PlayPortalSound() {
        _sfxSource.PlayOneShot(_portalSoundClip);
    }

    public void SetMasterVolume(float value) {
        MasterVolume = value;
        _audioMixer.SetFloat("Master", value < 0.001 ? -80 : Mathf.Log10(value) * 20);
    }

    public void SetBGMVolume(float value) {
        BGMVolume = value;
        _audioMixer.SetFloat("BGM", value < 0.001 ? -80 : Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value) {
        SFXVolume = value;
        _audioMixer.SetFloat("SFX", value < 0.001 ? -80 : Mathf.Log10(value) * 20);
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    
    public static AudioMixerController Instance { get; private set; }
    
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public Slider musicMasterSlider;
    [SerializeField] private Slider musicBoss1Slider;
    [SerializeField] private Slider musicBGMSlider;
    
    [SerializeField] private AudioClip[] mPreloadClips;
    private Dictionary<string, AudioClip> mClipsDictionary;
    [SerializeField] AudioSource mAudioSource;
    
    
    const string Master  = "Master";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 인스턴스 설정
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        musicBoss1Slider.onValueChanged.AddListener(SetBoss1Volume);
        
        mClipsDictionary = new Dictionary<string, AudioClip>();
        //
        foreach (AudioClip clip in mPreloadClips)
        {
            mClipsDictionary.Add(clip.name, clip);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 이름: {scene.name}, 로드 모드: {mode}");
        
        // 실행할 코드 추가
        if (scene.name == "UIScene")
        {
            mAudioSource.Stop();
            StartClip("Funky Chill 2 loop");
        }else if (scene.name == "SKW")
        {
            mAudioSource.Stop();
            // StopClip("Funky Chill 2 loop");
            StartClip("CGM3_Small_Quest_Complete_02");
        }
    }

    private void Start()
    {
      
        // 씬이 로드될 때 이벤트 등
    }

    private void Update()
    {
        Debug.Log(musicMasterSlider.value);
    }

    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = mClipsDictionary[clipName];

        //만약에 틀리면 없어요 
        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다."); }

        return clip;
    }
    
    public void StartClip(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip == null) { return; }
        mAudioSource.clip = clip;
        mAudioSource.PlayOneShot(clip);
    }

    public void StopClip(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip == null) { return; }
        mAudioSource.clip = clip;
        mAudioSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(Master,  Mathf.Log10(volume) * 20);
        Debug.Log(volume);
    }
    
 
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
 
    public void SetBoss1Volume(float volume)
    {
        audioMixer.SetFloat("Boss1",  Mathf.Log10(volume) * 20);
    }
    
}

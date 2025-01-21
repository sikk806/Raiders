using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    
    public static AudioMixerController Instance { get; private set; }
    
    
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public Slider musicMasterSlider;
    [SerializeField] private Slider musicBoss1Slider;
    [SerializeField] private Slider musicBGMSlider;
    
    [SerializeField] private AudioClip[] MainMenuClips;
    [SerializeField] private AudioClip[] PlayerTestClips;
    //메인메뉴 딕셔너리
    private Dictionary<string, AudioClip> mClipsDictionary;
    //Boss1 딕셔너리
    private Dictionary<string, AudioClip> b1ClipsDictionary;
    //Boss2 딕셔너리
    private Dictionary<string, AudioClip> b2ClipsDictionary;
    //PlayerTerst 딕셔너리
    private Dictionary<string, AudioClip> pClipsDictionary;
    [SerializeField] AudioSource mAudioSource;
    
    //값만 던지고 silder같은 할당은 로비씬에서만 해도 되지않을까?
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
        
        musicMasterSlider.onValueChanged.AddListener((value) => SetVolume(DefineMusicName.Master, value));
        musicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        musicBoss1Slider.onValueChanged.AddListener(SetBoss1Volume);
        
        mClipsDictionary = new Dictionary<string, AudioClip>();
        pClipsDictionary = new Dictionary<string, AudioClip>();
        // foreach (AudioClip clip in PlayerTestClips)
        // {
        //     pClipsDictionary.Add(clip.name, clip);
        // }
        AddClip(MainMenuClips,mClipsDictionary);
        AddClip(PlayerTestClips,pClipsDictionary);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AddClip(AudioClip[] clipName,Dictionary<string,AudioClip> clipDictionary)
    {
        foreach (AudioClip clip in clipName)
        {
            clipDictionary.Add(clip.name, clip);
        }
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
            StartClip(mClipsDictionary,"Funky Chill 2 loop");
        }else if (scene.name == "PlayerTest")
        {
            mAudioSource.Stop();
            // StopClip("Funky Chill 2 loop");
            StartClip(pClipsDictionary,"FA_Win_Jingle_Loop");
            mAudioSource.loop = true;
        }
    }



   

    private AudioClip GetClip(Dictionary<string,AudioClip> dictionary, string clipName)
    {
        AudioClip clip = dictionary[clipName];

        //만약에 틀리면 없어요 
        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다."); }

        return clip;
    }
    
    public void StartClip(Dictionary<string,AudioClip> dictionary,string clipName)
    {
        AudioClip clip = GetClip(dictionary,clipName);
        if (clip == null) { return; }
        mAudioSource.clip = clip;
        mAudioSource.Play();
    }

    // public void StopClip(string clipName)
    // {
    //     AudioClip clip = GetClip(clipName);
    //     if (clip == null) { return; }
    //     mAudioSource.clip = clip;
    //     mAudioSource.Stop();
    // }

    public void SetVolume(string musicname, float volume)
    {
        audioMixer.SetFloat(musicname, Mathf.Log10(volume) * 20);
        Debug.Log(musicname);
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

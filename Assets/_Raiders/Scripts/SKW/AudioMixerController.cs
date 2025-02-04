using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum SceneType
{
    MainScene,
    LobbyScene,
    Boss1Scene,
    Boss2Scene,
    Other
}

public class AudioMixerController : MonoBehaviour
{

    public static AudioMixerController Instance { get; private set; }


    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public Slider musicMasterSlider;
    [SerializeField] private Slider musicBoss1Slider;
    [SerializeField] private Slider musicBGMSlider;

    [SerializeField] private AudioClip[] MainMenuClips;
    [SerializeField] private AudioClip[] PlayerTestClips;
    [SerializeField] private AudioClip[] BossClips;
    //뱀파이어 클리스트 
    //뱀파이어 딕셔너리 

    //메인메뉴 딕셔너리
    private Dictionary<string, AudioClip> mClipsDictionary;
    //Boss 딕셔너리
    private Dictionary<string, AudioClip> bClipsDictionary;
    //PlayerTerst 딕셔너리
    private Dictionary<string, AudioClip> pClipsDictionary;
    [SerializeField] AudioSource mAudioSource;
    [SerializeField] AudioSource bAudioSource;

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

        //메인 메뉴에 해당하는 사운드 딕셔너리
        //PLAYERTEST에 해당하는 딕셔너리 
        mClipsDictionary = new Dictionary<string, AudioClip>();
        bClipsDictionary = new Dictionary<string, AudioClip>();
        pClipsDictionary = new Dictionary<string, AudioClip>();
        AddClip(MainMenuClips, mClipsDictionary);
        AddClip(PlayerTestClips, pClipsDictionary);
        AddClip(BossClips, bClipsDictionary);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AddClip(AudioClip[] clipName, Dictionary<string, AudioClip> clipDictionary)
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


    private SceneType GetSceneType(string sceneName)
    {
        return sceneName switch
        {
            "MainScene" => SceneType.MainScene,
            "LobbyScene" => SceneType.LobbyScene,
            "Boss1Scene" => SceneType.Boss1Scene,
            "Boss2Scene" => SceneType.Boss2Scene,
            _ => SceneType.Other,
        };
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 이름: {scene.name}, 로드 모드: {mode}");

        SceneType sceneType = GetSceneType(scene.name);

        switch (sceneType)
        {
            case SceneType.MainScene:
                mAudioSource.Stop();
                StartClip(mClipsDictionary, "Funky Chill 2 loop");
                break;

            case SceneType.LobbyScene:
                Debug.Log("소리 넣을꺼 확인");
                break;
            case SceneType.Boss1Scene:
                mAudioSource.Stop();
                StartClip(pClipsDictionary, "FA_Win_Jingle_Loop");
                mAudioSource.loop = true;
                // 다른 씬에 대한 처리
                break;
            case SceneType.Boss2Scene:
                // 다른 씬에 대한 처리
                Debug.Log("소리 넣을꺼 확인");
                break;
        }
    }





    private AudioClip GetClip(Dictionary<string, AudioClip> dictionary, string clipName)
    {
        AudioClip clip = dictionary[clipName];

        //만약에 틀리면 없어요 
        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다."); }

        return clip;
    }

    public void StartClip(Dictionary<string, AudioClip> dictionary, string clipName)
    {
        AudioClip clip = GetClip(dictionary, clipName);
        if (clip == null) { return; }
        mAudioSource.clip = clip;
        mAudioSource.Play();
    }

    public void BossStartClip(string clipName)
    {
        AudioClip clip = GetClip(bClipsDictionary, clipName);
        if (clip == null) { return; }
        bAudioSource.clip = clip;
        bAudioSource.Play();
    }

    public void SetVolume(string musicname, float volume)
    {
        audioMixer.SetFloat(musicname, Mathf.Log10(volume) * 25);
        Debug.Log(musicname);
    }


    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetBoss1Volume(float volume)
    {
        audioMixer.SetFloat("Boss1", Mathf.Log10(volume) * 20);
    }

}

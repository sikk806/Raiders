using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject PauseMenu;
    public bool IsOpen = false;
    public GameObject OptionsMenu;
    
    
    void Awake()
    {
         
        // 이미 다른 인스턴스가 존재하면 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스 설정
        Instance = this;

        // 이 오브젝트를 씬 전환 시에도 유지
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(PauseMenu);
        // DontDestroyOnLoad(OptionsMenu);

        // 현재 씬 안에 로드된 모든 객체(Canvas) 확인 setacitv false true건 찾는다.
    }
    
    private void Update()
    {
        PauseGame();
    }

    //씬이름이 있는지 없는지 확인법
    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path); // 경로에서 씬 이름만 추출
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    
    
    public void LoadSceneByName(string sceneName)
    {
        // 씬 이름이 null 또는 빈 문자열인지 확인
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }
        // 씬 이름이 빌드 설정에 포함되어 있는지 확인
        if (!IsSceneInBuild(sceneName))
        {
            return;
        }
        
        SceneManager.LoadScene(sceneName);
    }
    
    
    //게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }

    //현재 MainMenu는 UI씬으로 테스트중
    public void ReturnToMainMenu()
    {
        //메인씬 이동 후
        //타임스케일 정상화
        SceneManager.LoadScene(0);
        ResetTimeScale();
    }

    //일시정지를 위한 코드 
    //근데 이걸 메인메뉴에서부터 싱글턴으로 가져갈 이유가 있을까 ?
    public void PauseGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (currentScene.name == "UIScene")
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (IsOpen)
            {
                ClosePauseMenu();
            }
            else if (!IsOpen)
            {
                ShowPauseMenu();
            }
        }
    }

    public void ShowPauseMenu()
    {
        IsOpen = true;
        PauseMenu.SetActive(true); 
        Time.timeScale = 0;
    }
    public void ClosePauseMenu()
    {
        IsOpen = false;
        PauseMenu.SetActive(false);
        ResetTimeScale();
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    public void OpenOptionsMenu()
    {
        OptionsMenu.SetActive(true);
    }




}

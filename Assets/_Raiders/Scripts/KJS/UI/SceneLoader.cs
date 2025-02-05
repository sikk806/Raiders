using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public Image FadeCanvas;
    public TMP_Text Description;
    public Slider ProgressBar;
    public TMP_Text Percentage;

    float fadeDuration = 1.0f;
    List<String> descriptions = new List<string>
    {
        "보스 클리어를 응원... 해드려야 하나요?",
        "이걸 도전해 보신다구요? 대단하시네요.",
        "자, 마우스를 잡고 왼쪽 버튼을 눌러보세요. 이게 클릭이랍니다.",
        "어.. 음.. 네.. 열심히 잡아보세요..!",
        "(뿡.) 앗.. 실수ㅎ",
        "분수대 근처에서 상호작용 키를 누르면 패시브를 얻을 수 있어요!\n그런데 패시브 없이 싸울 수 없나요...? 저런...",
        "이렇게 물불 안 가리는 녀석은 내 스타일이 아닌데.",
        "응원합니다! (진심X)"
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (FadeCanvas == null) return; // 안전 체크
        FadeCanvas.color = new Color(0, 0, 0, 1); // 시작 시 완전 검은 화면
        ProgressBar.gameObject.SetActive(false);
        StartCoroutine(FadeIn());
    }

    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        FadeCanvas.gameObject.SetActive(true);
        ProgressBar.gameObject.SetActive(true);
        yield return StartCoroutine(FadeOut());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            ProgressBar.value = asyncLoad.progress;
            Percentage.text = (asyncLoad.progress * 100).ToString() + "%";
            yield return null;
        }

        ProgressBar.value = 1f;
        Percentage.text = "100%";
        yield return new WaitForSeconds(1f);

        Description.text = "";
        Percentage.text = "";
        ProgressBar.gameObject.SetActive(false);
        asyncLoad.allowSceneActivation = true;
        yield return null;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (FadeCanvas == null) yield break; // 안전 체크
        Color color = FadeCanvas.color;
        for (float t = fadeDuration; t >= 0; t -= Time.deltaTime)
        {
            color.a = Mathf.Clamp01(t / fadeDuration); // 보정 추가
            FadeCanvas.color = color;
            yield return null;
        }
        color.a = 0; // 최종 보정
        FadeCanvas.gameObject.SetActive(false);
        FadeCanvas.color = color;
    }

    IEnumerator FadeOut()
    {
        if (FadeCanvas == null) yield break; // 안전 체크

        ProgressBar.value = 0f;
        Percentage.text = "0%";
        Description.text = descriptions[Random.Range(0, 8)];
        Color color = FadeCanvas.color;
        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Clamp01(t / fadeDuration); // 보정 추가
            FadeCanvas.color = color;
            yield return null;
        }
        color.a = 1; // 최종 보정
        FadeCanvas.color = color;
    }
}

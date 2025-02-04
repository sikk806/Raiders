using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public Image FadeCanvas;
    float fadeDuration = 1.0f;

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
        StartCoroutine(FadeIn());
    }

    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        FadeCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeOut());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

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

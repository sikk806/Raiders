using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance { get { Init(); return instance; } }

    [SerializeField] GameObject uDieUI;
    [SerializeField] TMP_Text DieText;
    [SerializeField] GameObject revUI;
    [SerializeField] TMP_Text countDown;
    InputManager input;
    public static InputManager Input { get { return Instance.input; } }
    SkillManager skill;
    // 2025-01-23 static 제거
    public SkillManager Skill { get { return Instance.skill; } }

    private float deathCount = 5;
    public float DeathCount { get { return deathCount; } set { deathCount = value; } }

    private float playTime = 240f;
    public float PlayTime { get { return playTime; } set { PlayTime = value; } }

    public Action WinAction;

    bool checkClear;

    private void Awake()
    {
        input = new InputManager();
        skill = FindAnyObjectByType<SkillManager>();

        checkClear = false;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        input.OnUpdate();

        // 시간 재는 코드 추가 필요 (보스 씬에 넘어가는 순간 체크 시작)
    }

    public void DeathCountDown()
    {
        deathCount--;

        if (deathCount <= 0)
        {
            Debug.Log("GameOver");
            // DeathCount가 0이 됐을 때 UDie UI 출력
            StartCoroutine("UDie");
        }
        else
        {
            StartCoroutine("RevPlayer");
        }
    }

    public void NegaitveGameTime()
    {
        playTime -= Time.deltaTime;
    }

    public string makeTime()
    {
        int hours = (int)(playTime / 3600); // 시간 계산
        int minutes = (int)(playTime % 3600) / 60; // 분 계산
        int seconds = (int)(playTime % 60); // 초 계산
        
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
    static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("GameManager");
            if (go == null)
            {
                go = new GameObject { name = "GameManager" };
                go.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<GameManager>();
        }

    }

    IEnumerator UDie()
    {
        DieText.text = "못 깼죠? ㅋ";
        DieText.color = Color.red;
        DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 1f);
        DieText.fontSize = 60f;
        uDieUI.SetActive(true);

        Image UIImage = uDieUI.GetComponent<Image>();

        float Timer = 0f;

        while (Timer < 2f)
        {
            Timer += Time.deltaTime;
            UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, UIImage.color.a + 0.02f * Time.deltaTime);
            DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, DieText.color.a + 0.05f * Time.deltaTime);
            DieText.fontSize += 1f;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        DieText.fontSize = 60f;
        uDieUI.SetActive(false);
        UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, 0f);
        Destroy(GameObject.FindWithTag("Player"));
        GameObject[] objects = GameObject.FindGameObjectsWithTag("NotInTitle");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        DeathCount = 5;
        SceneLoader.Instance.LoadNewScene("LobbyScene");
    }

    public void WinTheGame()
    {
        if(!checkClear) StartCoroutine("WinGame");
    }

    IEnumerator WinGame()
    {
        checkClear = true;
        uDieUI.SetActive(true);
        DieText.text = "ㅅ..승리.....";
        DieText.color = Color.blue;
        Image UIImage = uDieUI.GetComponent<Image>();
        UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, 0f);
        DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 0f);
        DieText.fontSize = 60f;


        float Timer = 0f;

        while (Timer < 1f)
        {
            Timer += Time.deltaTime;
            UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, UIImage.color.a + 0.002f);
            DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, DieText.color.a + 0.005f);
            DieText.fontSize += 1.5f * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        DieText.fontSize = 60f;
        uDieUI.SetActive(false);
        UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, 0f);
        Destroy(GameObject.FindWithTag("Player"));
        GameObject[] objects = GameObject.FindGameObjectsWithTag("NotInTitle");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        DeathCount = 5;
        SceneLoader.Instance.LoadNewScene("MainScene");
    }

    IEnumerator RevPlayer()
    {
        revUI.SetActive(true);
        float revTime = 5f;
        Slider slider = revUI.GetComponent<Slider>();
        slider.value = 1f;
        while (revTime > 0.1f)
        {
            revTime -= Time.deltaTime;
            slider.value = revTime / 5f;
            countDown.text = Math.Round(revTime) + "초 후 부활합니다.";
            yield return null;
        }
        revUI.SetActive(false);
        Player.Instance.GetComponent<Hp>().Resurrection();
    }
}

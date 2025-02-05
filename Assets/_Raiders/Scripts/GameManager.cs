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

    InputManager input;
    public static InputManager Input { get { return Instance.input; } }
    SkillManager skill;
    // 2025-01-23 static 제거
    public SkillManager Skill { get { return Instance.skill; } }

    private float deathCount = 1;
    public float DeathCount { get { return deathCount; } set { deathCount = value; } }

    private float playTime;
    public float PlayTime { get { return playTime; } set { PlayTime = value; } }

    public Action WinAction;

    private void Awake()
    {
        input = new InputManager();
        skill = FindAnyObjectByType<SkillManager>();
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

        if (deathCount < 0)
        {
            Debug.Log("GameOver");
            // DeathCount가 0이 됐을 때 UDie UI 출력
            UDie();
        }
        else
        {
            Debug.Log("Resurrection after 5 sec, use popup and call Function (Player.Instance.Resurrection)");
        }
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
        DieText.text = "유  다  희";
        DieText.color = Color.red;
        DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 0f);
        DieText.fontSize = 60f;
        uDieUI.SetActive(true);

        Image UIImage = uDieUI.GetComponent<Image>();

        while(UIImage.color.a <= 0.4f)
        {
            UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, UIImage.color.a + 0.02f);
            DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 0.05f);
            DieText.fontSize += 1.5f;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        DieText.fontSize = 60f;
        uDieUI.SetActive(false);
        UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, 0f);
        Destroy(GameObject.FindWithTag("Player"));
        DeathCount = 5;
        SceneLoader.Instance.LoadNewScene("LobbyScene");
    }

    public void WinTheGame()
    {
        StartCoroutine("WinGame");
    }

    IEnumerator WinGame()
    {
        DieText.text = "ㅅ..승리.....";
        DieText.color = Color.blue;
        DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 0f);
        DieText.fontSize = 60f;
        uDieUI.SetActive(true);

        Image UIImage = uDieUI.GetComponent<Image>();

        while(UIImage.color.a <= 0.4f)
        {
            UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, UIImage.color.a + 0.02f);
            DieText.color = new Color(DieText.color.r, DieText.color.g, DieText.color.b, 0.05f);
            DieText.fontSize += 1.5f;
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        DieText.fontSize = 60f;
        uDieUI.SetActive(false);
        UIImage.color = new Color(UIImage.color.r, UIImage.color.g, UIImage.color.b, 0f);
        Destroy(GameObject.FindWithTag("Player"));
        DeathCount = 5;
        SceneLoader.Instance.LoadNewScene("MainScene");
    }
}

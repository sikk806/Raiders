using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance { get { Init(); return instance; } }

    InputManager input;
    public static InputManager Input { get { return Instance.input; } }
    SkillManager skill;
    // 2025-01-23 static 제거
    public SkillManager Skill { get { return Instance.skill; } }

    private float deathCount = 5;
    public float DeathCount { get { return deathCount; } set { deathCount = value; }}

    private float playTime;
    public float PlayTime { get { return playTime; } set { PlayTime = value; }}

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

        if (deathCount < 0 )
        {
            Debug.Log("GameOver");
            // DeathCount가 0이 됐을 때 UDie UI 출력
            // 코루틴으로 넘길 것 (3초정도)
            Destroy(GameObject.FindWithTag("Player"));
            SceneLoader.Instance.LoadNewScene("로비 씬으로 넘기기.");
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
}

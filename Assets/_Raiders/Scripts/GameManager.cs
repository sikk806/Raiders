using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance { get { Init(); return instance; } }

    InputManager input;
    public static InputManager Input { get { return Instance.input; } }
    SkillManager skill;
    public static SkillManager Skill { get { return Instance.skill; } }

    private float deathCount = 5;

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
    }

    public void DeathCountDown()
    {
        deathCount--;

        if (deathCount < 0 )
        {
            Debug.Log("GameOver");
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

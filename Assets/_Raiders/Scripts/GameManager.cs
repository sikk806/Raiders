using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance { get { Init(); return instance; } }

    InputManager input;
    public static InputManager Input { get { return Instance.input; } }

    private float deathCount = 5; //데스 카운트(임시 생성)

    private void Awake()
    {
        input = new InputManager();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        input.OnUpdate();
    }

    public void DeathCountDown() //데스카운트 감소 처리
    {
        //데스카운트 1 감소
        deathCount--;

        //데스카운트가 0 미만이면
        if (deathCount < 0 )
        {
            //게임 종료
            Debug.Log("GameOver");
        }
        else
        {
            //5초 후 부활 안내 팝업 띄운 이후,캐릭터 부활
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

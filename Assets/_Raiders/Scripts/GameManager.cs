using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    static private GameManager instance = null;
    static public GameManager Instance { get { Init(); return instance; } }

    InputManager input;
    public static InputManager Input { get { return Instance.input; } }

    private float deathCount = 5; 

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

    public void DeathCountDown() //����ī��Ʈ ���� ó��
    {
        //����ī��Ʈ 1 ����
        deathCount--;

        //����ī��Ʈ�� 0 �̸��̸�
        if (deathCount < 0 )
        {
            //���� ����
            Debug.Log("GameOver");
        }
        else
        {
            //5�� �� ��Ȱ �ȳ� �˾� ��� ����,ĳ���� ��Ȱ
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

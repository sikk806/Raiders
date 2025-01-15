using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using static Player;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public enum PlayerState //�÷��̾� ����
{
    Idle,
    Move,
    Roll,
    Attack,
    Null,
    UseQ,
    UseW,
    UseE,
    UseR
}

public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public float MaxHp = 100; //�ִ�HP
    public float CurrentHp = 100; //����HP
    public float MaxMp = 1000; //�ִ�MP
    public float CurrentMp = 1000; //����MP

    public float Power = 100; //���ݷ�
    public float AddedPower = 0; //�߰� ���ݷ�

    private Vector3 targetPosition; //�̵� ��ǥ
    private LayerMask stage; //���� ���̾��ũ
    public float MoveSpeed = 7f; //�̵� �ӵ�
    public float RollingDistance = 5f; //������ �Ÿ�
    public float RollingTime = 0.35f; //������ �ð�


    public float StopTime; //���� �ð�
    public float Barrier; //���差
    public float NoDamageTime = 1f; //���� �ð�

    private bool IsRolling = false; //������ ����
    private bool IsNoDamaged = false; //���� ����

    [SerializeField]
    private GameObject AttackPrefab; //��Ÿ ������
    [SerializeField]
    private GameObject qPrefab; //Q ��ų ������
    [SerializeField]
    private GameObject wPrefab; //W ��ų ������
    [SerializeField]
    private GameObject ePrefab; //E ��ų ������
    [SerializeField]
    private GameObject rPrefab; //R ��ų ������
    [SerializeField]
    private GameObject attackPosition; //���� ������ ��ġ

    private Animator animator; 
    private InputManager inputManager;
    public PlayerState CurrentState; //���� ����


    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
        inputManager = GameManager.Input;

        //Ű�׼� ���� ���� (���� �ߺ� ������)
        TakeControl();

        //Ű�׼� ����
        BringBackControl();

        //�������� �浹 ���̾� ����
        stage = LayerMask.GetMask("Stage");
    }
    IEnumerator NoDamage() //���� �ο� �ڷ�ƾ
    {
        //�̹� ���� ���¶��, �ڷ�ƾ ����
        if (IsNoDamaged) yield break;

        //���� ���·� ����
        IsNoDamaged = true;

        //NoDamageTime��ŭ ���� ���� ����
        yield return new WaitForSeconds(NoDamageTime);
        
        //���� ���� ����
        IsNoDamaged = false;
    }


    public void Damaged(float damage) //�޴� ������ ó��
    {
        //���� ���¶�� ó������ ����
        if (IsNoDamaged) { return; }

        //Hp�� ��������ŭ �پ��
        CurrentHp -= damage;

        //Hp ���� ���� ó��
        CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        //Hp�� 0 ���϶��
        if (CurrentHp <= 0f)
        {
            //���� ó�� �޼��� ȣ��
            Die();
        }

    }

    public void Die() //���� ó��
    {
        //���� �÷��̾� ���¸� Null�� ����
        CurrentState = PlayerState.Null;

        //Ű�׼� ���� ����
        TakeControl();

        //����ī��Ʈ ����
        GameManager.Instance.DeathCountDown();
        
        //Death �ִϸ��̼� ���
        animator.Play("Death");
    }

    public void Stuned(float stunTime) //���� ó��
    {
        //���� ���¶�� ó������ ����
        if (IsNoDamaged) { return; }

        //���� �÷��̾� ���¸� Null�� ����
        CurrentState = PlayerState.Null;

        //Ű�׼� ���� ����
        TakeControl();
        
        //Stun �ִϸ��̼� ���
        animator.Play("Stun");

        //���� �ð���ŭ ��ٷȴٰ�, Ű�׼� �籸��
        Invoke("BringBackControl", stunTime);

    }

    public void Resurrection() //��Ȱ ó��
    {
        //���� �ο�
        StartCoroutine(NoDamage());

        //Ű�׼� �籸��
        BringBackControl();

        //�ִ� Hp �ʱ�ȭ
        MaxHp = 100f;
        
        //�ִ� Mp �ʱ�ȭ
        MaxMp = 1000f;

        //���� Hp �ʱ�ȭ
        CurrentHp = MaxHp;

        //���� Mp �ʱ�ȭ
        CurrentMp = MaxMp;

        //���� �÷��̾� ���¸� Idle�� ����
        CurrentState = PlayerState.Idle;
    }

    void TakeControl() //ĳ���� ���� �Ұ� ó��
    {
        //Ű�׼� ���� ����
        GameManager.Input.KeyAction -= OnMouse;
        GameManager.Input.KeyAction -= OnKeyboard;
    }

    void BringBackControl() //ĳ���� ���� ���� ó�� 
    {
        //Ű�׼� ����
        GameManager.Input.KeyAction += OnMouse;
        GameManager.Input.KeyAction += OnKeyboard;

    }
    void Update()
    {
        //���� �÷��̾� ���¿� ���� �ִϸ��̼� ���� ó��
        switch (CurrentState) 
        {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Move:
                HandleMove();
                break;
            case PlayerState.Roll:
                HandleRoll();
                break;
            case PlayerState.Attack:
                HandleAttack();
                break;
            case PlayerState.Null:
                HandleNull();
                break;
            case PlayerState.UseQ:
                HandleUseQ();
                break;
            case PlayerState.UseW:
                HandleUseW();
                break;
            case PlayerState.UseE:
                HandleUseE();
                break;
            case PlayerState.UseR:
                HandleUseR();
                break;
        }
    }

    void HandleIdle() //Idle ���� ó��
    {
        //�ִϸ��̼� Idle ���
        animator.Play("Idle");
    }

    void HandleMove() //Move ���� ó��
    {
        //�ִϸ��̼� Move ���
        animator.Play("Move");

        //�ٶ� ���� ���� ���
        Vector3 direction = (targetPosition - transform.position).normalized;

        //��ǥ ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        //�÷��̾ �ٶ� ���������� ȸ�� ó��
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, MoveSpeed / 2 * Time.deltaTime);
        }

        //�÷��̾ ��ǥ ������ �����Ѵٸ�
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            //���� �÷��̾� ���¸� Idle�� ����
            CurrentState = PlayerState.Idle;
        }
    }

    void HandleRoll() //Roll ���� ó��
    {
        //�ִϸ��̼� Roll ���
        animator.Play("Roll");

        //��ǥ ��ġ�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (RollingDistance / RollingTime) * Time.deltaTime);

        //�÷��̾ ��ǥ ������ �����Ѵٸ�
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            //������ ���� ����
            IsRolling = false;

            //���� �÷��̾� ���¸� Idle�� ����
            CurrentState = PlayerState.Idle;
        }

    }

    void HandleAttack() //Attack ���� ó��
    {
        //���� �ִϸ��̼� ���¸� ������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �ִϸ��̼� ���� �̸��� Attack�̸�
        if (stateInfo.IsName("Attack"))
        {
            //�ִϸ��̼��� ����Ǹ�
            if (stateInfo.normalizedTime >= 1f)
            {
                //���� �÷��̾� ���¸� Idle�� ����
                CurrentState = PlayerState.Idle;
            }
        }
    }

    void HandleNull() //Null ���� ó��
    {
        //�������
    }

    void HandleUseQ() //UseQ ���� ó�� 
    {
        //���� �ִϸ��̼� ���¸� ������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �ִϸ��̼� ���� �̸��� Q���
        if (stateInfo.IsName("Q"))
        {
            //�ִϸ��̼��� ����Ǹ�
            if (stateInfo.normalizedTime >= 1f)
            {
                //Ű�׼� ����
                BringBackControl();

                //���� �÷��̾� ���¸� Idle�� ����
                CurrentState = PlayerState.Idle;
            }
        }
    }
    
    void HandleUseW() //UseW ���� ó��
    {

    }

    void HandleUseE() //UseE ���� ó��
    {

    }

    void HandleUseR() //UseR ���� ó��
    {
        
    }

    void OnMouse() //���콺 �Է� ó��
    {
        //[Move]
        //��Ŭ�� ���� ��, ������ ���°� �ƴϸ�
        if (Input.GetMouseButton(1) && !IsRolling)
        {
            //���콺 ��ġ ���ϱ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //���̰� stage ���̾� ������Ʈ�� �浹�ߴٸ�
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {
                //���̰� �浹�� ������Ʈ �±װ� Stage���
                if (hit.collider.tag == "Stage")
                {
                    //��ǥ ��ġ�� �浹 �������� ����
                    targetPosition = hit.point;
                    
                    //��ǥ ��ġ�� �ٶ�
                    transform.LookAt(targetPosition);

                    //���� �÷��̾� ���¸� Move�� ����
                    CurrentState = PlayerState.Move;
                }
            }
        }

        //[Attack]
        //��Ŭ�� ���� ��, ��Ÿ ��� �����̸�
        if (Input.GetMouseButtonDown(0) && AutoAttack.Instance.IsUsable)
        {
            //���� �÷��̾� ���°� Idle �Ǵ� Move���
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //���� �÷��̾� ���¸� Attack���� ����
                CurrentState = PlayerState.Attack;

                //Attack �ִϸ��̼� Ʈ���� ����
                animator.SetTrigger("Attack");

                //���콺 ��ġ ���ϱ�
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //���̰� stage ���̾� ������Ʈ�� �浹�ߴٸ�
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
                {
                    //��ǥ ��ġ�� �浹 �������� ����
                    targetPosition = hit.point;

                    //��ǥ ��ġ�� �ٶ�
                    transform.LookAt(targetPosition);

                    //��Ÿ �ν��Ͻ� ����
                    GameObject autoAttack = Instantiate(AttackPrefab, attackPosition.transform);
                    
                    //1�� �ڿ� ��Ÿ �ν��Ͻ� ����
                    Destroy(autoAttack, 1f);
                }
            }

        }
    }

    void OnKeyboard() //Ű���� �Է� ó��
    {
        //[Roll]
        if (Input.GetKey(KeyCode.Space)) 
        {
            //���콺 ��ġ ���ϱ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //���̰� stage ���̾� ������Ʈ�� �浹�ߴٸ�
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {

                //���̰� �浹�� ������Ʈ �±װ� Stage���
                if (hit.collider.tag == "Stage")
                {
                    //��ǥ ��ġ�� �浹 �������� ����
                    targetPosition = hit.point;

                    //�ٶ� ��ġ ����
                    Vector3 lookDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
                    
                    //������ ��ġ�� �ٶ�
                    transform.LookAt(lookDirection);

                    //������ ���� Ȱ��ȭ
                    IsRolling = true;

                    //���� �ο�
                    StartCoroutine(NoDamage());

                    //���� �÷��̾� ���¸� Roll�� ����
                    CurrentState = PlayerState.Roll;
                }
            }
        }

        //[Use Q]
        if (Input.GetKey(KeyCode.Q))
        {
            //���� �÷��̾� ���°� Idle �Ǵ� Move���
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //Q ��� �����̸�, ���� Mp�� ��ų �Ҹ� Mp �̻��̶��
                if (Q.Instance.IsUsable && CurrentMp >= Q.Instance.UseMp)
                {
                    //Q ��ų ���
                    Q.Instance.UseSkill();

                    //���� �÷��̾� ���¸� UseQ�� ����
                    CurrentState = PlayerState.UseQ;

                    //Q �ִϸ��̼� Ʈ���� ���� 
                    animator.SetTrigger("Q");

                    //Q �ν��Ͻ� ����
                    GameObject q = Instantiate(qPrefab, attackPosition.transform);
                    
                    //Ű�׼� ���� ����
                    TakeControl();

                    //Q �ν��Ͻ� 2�� �� ����
                    Destroy(q, 2f);
                }

                //Q ��ų ��� �Ұ� ���¶��
                else if (!Q.Instance.IsUsable)
                {
                    Debug.Log("���� ��ų�� �� �� ���ٴ� �޽��� ����");
                }
               
                //���� Mp�� ��ų �Ҹ� Mp �̸��̶��
                else if (CurrentMp < Q.Instance.UseMp)
                {
                    Debug.Log("MP ���� �޽��� ����");
                }
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            W.Instance.UseSkill(); //���� �پ��� �� ���ư��⸸ �ϴ� ��
            //�ٶ� �������� ��ų�� ��ġ, ���� �� ��� ������ ����� �ο�
        }

        if (Input.GetKey(KeyCode.E))
        {
            E.Instance.UseSkill();
            //4�ʰ� ���ӵǴ� �ǵ� �����, �ǵ�� Hp���� �켱�ؼ� �����ϱ� Damaged������ �۾��������
        }

        if (Input.GetKey(KeyCode.R))
        {
            R.Instance.UseSkill();
            //��ų ���� �� ��� ������ ����� �ο�, ���� �ð����� ���� 

        }
    }
}




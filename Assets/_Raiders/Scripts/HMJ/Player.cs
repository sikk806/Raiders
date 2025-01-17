using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    public float RollingDistance = 8.5f; //������ �Ÿ�
    public float RollingTime = 0.617f; //������ �ð�


    public float StopTime; //���� �ð�
    public float Barrier; //���差
    public float NoDamageTime = 1f; //���� �ð�
    private float movementSpeedRatio = 0;


    private bool IsRolling = false; //������ ����
    private bool IsNoDamaged = false; //���� ����

    public Image HpBar;
    public Image MpBar;
    public TextMeshProUGUI HpText; //Hp�� �ؽ�Ʈ
    public TextMeshProUGUI MpText; //Mp�� �ؽ�Ʈ

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
        HpText.text = CurrentHp + "/" + MaxHp;
        MpText.text = CurrentMp + "/" + MaxMp;


        //Ű�׼� ���� ���� (���� �ߺ� ������)
        TakeControl();

        //Ű�׼� ����
        BringBackControl();

        //�������� �浹 ���̾� ����
        stage = LayerMask.GetMask("Stage");

    }

    IEnumerator NoDamage(float noDamageTime) //���� �ο� �ڷ�ƾ
    {
        //�̹� ���� ���¶��, �ڷ�ƾ ����
        if (IsNoDamaged) yield break;

        //���� ���·� ����
        IsNoDamaged = true;

        //NoDamageTime��ŭ ���� ���� ����
        yield return new WaitForSeconds(noDamageTime);

        //���� ���� ����
        IsNoDamaged = false;
    }

    public void BarrierSet()
    {
        Barrier = 50f;
        HpText.text = CurrentHp + "(+" + Barrier + ")" + "/" + MaxHp;
    }

    public void BarrierReset() //���� ���� (����, Invoke ���ؼ� ȣ���ϴ� ������ �����ð� ����)
    {
        Barrier = 0f;
        HpText.text = CurrentHp + "/" + MaxHp;
    }


    public void Damaged(float damage) //�޴� ������ ó��
    {
        //���� ���¶�� ó������ ����
        if (IsNoDamaged) { return; }
        else if (Barrier > 0f)
        {
            float lastDamage = damage - Barrier;

            Barrier -= damage;
            Barrier = Mathf.Clamp(Barrier, 0f, Mathf.Infinity);

            if (lastDamage >= 0f)
            {
                damage = lastDamage;

                //Hp�� ��������ŭ �پ��
                CurrentHp -= damage;
                HpBar.fillAmount -= damage;
                HpText.text = CurrentHp + "/" + MaxHp;

                //Hp ���� ���� ó��
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp�� 0 ���϶��
                if (CurrentHp <= 0f)
                {
                    //���� ó�� �޼��� ȣ��
                    Die();
                }
            }
            else
            {
                //Hp�� ��������ŭ �پ��
                CurrentHp -= damage;
                HpBar.fillAmount = CurrentHp / MaxHp;

                //Hp ���� ���� ó��
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp�� 0 ���϶��
                if (CurrentHp <= 0f)
                {
                    //���� ó�� �޼��� ȣ��
                    Die();
                }
            }
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
        animator.SetTrigger("Death");
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
        animator.SetTrigger("Stun");

        //���� �ð���ŭ ��ٷȴٰ�, Ű�׼� �籸��
        Invoke("BringBackControl", stunTime);

    }

    public void Resurrection() //��Ȱ ó��
    {
        //���� �ο�
        StartCoroutine(NoDamage(NoDamageTime));

        //Ű�׼� �籸��
        BringBackControl();

        //�ִ� Hp �ʱ�ȭ
        MaxHp = 100f;

        //�ִ� Mp �ʱ�ȭ
        MaxMp = 1000f;

        //���� Hp �ʱ�ȭ
        CurrentHp = MaxHp;
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

        //���� Mp �ʱ�ȭ
        CurrentMp = MaxMp;
        MpBar.fillAmount = CurrentMp / MaxMp;
        MpText.text = CurrentMp + "/" + MaxMp;

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

        /*
         * �÷��̾��� ĳ���� ��Ʈ�� ������ ���� ���� ����, ���� ����� Update���� ó���մϴ�.
         */

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //(Hp���� ����) > 0f ���, Hp������ ����
            //else: ������ ����� �� �����ϴ�.
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //(Mp���� ����) > 0f ���, Mp������ ����
            //else: ������ ����� �� �����ϴ�.
        }

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
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 0, MoveSpeed * Time.deltaTime);
        animator.SetFloat("Move", movementSpeedRatio);
        //�ִϸ��̼� Idle 
        animator.Play("IdleMove");
    }

    void HandleMove() //Move ���� ó��
    {
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 1, MoveSpeed * Time.deltaTime);

        animator.SetFloat("Move",movementSpeedRatio);

        //�ִϸ��̼� Move ���
        animator.Play("IdleMove");

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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Roll"))
        {
            animator.Play("Roll");
        }
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
                //Ű�׼� ����
                BringBackControl();
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
        //���� �ִϸ��̼� ���¸� ������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �ִϸ��̼� ���� �̸��� W���
        if (stateInfo.IsName("W"))
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

    void HandleUseE() //UseE ���� ó��
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if(stateInfo.IsName("E"))
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                CurrentState = PlayerState.Idle;
            }
        }

    }

    void HandleUseR() //UseR ���� ó��
    {
        //���� �ִϸ��̼� ���¸� ������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �ִϸ��̼� ���� �̸��� R���
        if (stateInfo.IsName("R"))
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
                //Ű�׼� ���� ����
                TakeControl();

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
                    Destroy(autoAttack, 0.85f);
                }
            }

        }
    }

    void OnKeyboard() //Ű���� �Է� ó��
    {
        //[Roll]
        if (Input.GetKeyDown(KeyCode.Space) && !IsRolling)
        {
            //���콺 ��ġ ���ϱ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;          

            //���̰� stage ���̾� ������Ʈ�� �浹�ߴٸ�
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {
                // ���콺 ��ġ���� ���� �÷��̾� ��ġ������ ���� ���ϱ�
                Vector3 direction = (hit.point - transform.position).normalized;

                // ��ǥ ��ġ�� ���� ��ġ���� RollingDistance��ŭ �̵��� �������� ����
                targetPosition = transform.position + direction * RollingDistance;

                // �ٶ� ��ġ ���� (y ���� ���� �÷��̾��� y�� ����)
                Vector3 lookDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);


                //������ ��ġ�� �ٶ�
                transform.LookAt(lookDirection);

                //������ ���� Ȱ��ȭ
                IsRolling = true;

                //���� �ο�
                StartCoroutine(NoDamage(NoDamageTime));

                //���� �÷��̾� ���¸� Roll�� ����
                CurrentState = PlayerState.Roll;

            }
        }

        //[Use Q]
        if (Input.GetKeyDown(KeyCode.Q))
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

        //[UseW]
        if (Input.GetKeyDown(KeyCode.W))
        {
            //���� �÷��̾� ���°� Idle �Ǵ� Move���
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //W ��� �����̸�, ���� Mp�� ��ų �Ҹ� Mp �̻��̶��
                if (W.Instance.IsUsable && CurrentMp >= W.Instance.UseMp)
                {

                    //W ��ų ���
                    W.Instance.UseSkill();

                    //���� �÷��̾� ���¸� UseW�� ����
                    CurrentState = PlayerState.UseW;

                    //Q �ִϸ��̼� Ʈ���� ���� 
                    animator.SetTrigger("W");
                    
                    //Ű�׼� ���� ����
                    TakeControl();

                    //��ų ���� ��ġ ����
                    Vector3 wPosition = attackPosition.transform.position + attackPosition.transform.forward * 1f;
                    wPosition.y = 3f;

                    //W ��ų ��ġ
                    GameObject w = Instantiate(wPrefab, wPosition, transform.rotation);

                    //��ġ�� W ��ų 3�� �� ���� (TEMP)
                    Destroy(w, 3f);



                }
                //W ��ų ��� �Ұ� ���¶��
                else if (!W.Instance.IsUsable)
                {
                    Debug.Log("���� ��ų�� �� �� ���ٴ� �޽��� ����");
                }

                //���� Mp�� ��ų �Ҹ� Mp �̸��̶��
                else if (CurrentMp < W.Instance.UseMp)
                {
                    Debug.Log("MP ���� �޽��� ����");
                }
            }
        }

        //[UseE]
        if (Input.GetKeyDown(KeyCode.E))
        {
            //���� �÷��̾� ���°� Idle �Ǵ� Move���
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //E ��� �����̸�, ���� Mp�� ��ų �Ҹ� Mp �̻��̶��
                if (E.Instance.IsUsable && CurrentMp >= E.Instance.UseMp)
                {
                    //E ��ų ���
                    E.Instance.UseSkill();

                    //���� �÷��̾� ���¸� UseR�� ����
                    CurrentState = PlayerState.UseE;

                    BarrierSet();

                    //4�� �� ���差 0���� �ʱ�ȭ
                    Invoke("BarrierReset",4f);

                    //E �ִϸ��̼� Ʈ���� ���� 
                    animator.SetTrigger("E");

                    //E �ν��Ͻ� ����
                    GameObject e = Instantiate(ePrefab, attackPosition.transform);

                    //E �ν��Ͻ� 4�� �� ���� (TEMP)
                    Destroy(e, 4f);
                }

                //R ��ų ��� �Ұ� ���¶��
                else if (!R.Instance.IsUsable)
                {
                    Debug.Log("���� ��ų�� �� �� ���ٴ� �޽��� ����");
                }

                //���� Mp�� ��ų �Ҹ� Mp �̸��̶��
                else if (CurrentMp < R.Instance.UseMp)
                {
                    Debug.Log("MP ���� �޽��� ����");
                }
            }

        }

        //[UseR]
        if (Input.GetKeyDown(KeyCode.R))
        {
            //���� �÷��̾� ���°� Idle �Ǵ� Move���
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //R ��� �����̸�, ���� Mp�� ��ų �Ҹ� Mp �̻��̶��
                if (R.Instance.IsUsable && CurrentMp >= R.Instance.UseMp)
                {
                    //R ��ų ���
                    R.Instance.UseSkill();

                    //���� �÷��̾� ���¸� UseR�� ����
                    CurrentState = PlayerState.UseR;

                    //4�ʰ� ���� �ο� (TEMP)
                    NoDamage(4f);

                    //R �ִϸ��̼� Ʈ���� ���� 
                    animator.SetTrigger("R");

                    //R �ν��Ͻ� ����
                    GameObject r = Instantiate(rPrefab, attackPosition.transform);

                    //Ű�׼� ���� ����
                    TakeControl();

                    //R �ν��Ͻ� 4�� �� ���� (TEMP)
                    Destroy(r, 4f);
                }

                //R ��ų ��� �Ұ� ���¶��
                else if (!R.Instance.IsUsable)
                {
                    Debug.Log("���� ��ų�� �� �� ���ٴ� �޽��� ����");
                }

                //���� Mp�� ��ų �Ҹ� Mp �̸��̶��
                else if (CurrentMp < R.Instance.UseMp)
                {
                    Debug.Log("MP ���� �޽��� ����");
                }
            }
        }
    }
}





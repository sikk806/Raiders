using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using static Player;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public enum PlayerState //플레이어 상태
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
    
    public float MaxHp = 100; //최대HP
    public float CurrentHp = 100; //현재HP
    public float MaxMp = 1000; //최대MP
    public float CurrentMp = 1000; //현재MP

    public float Power = 100; //공격력
    public float AddedPower = 0; //추가 공격력

    private Vector3 targetPosition; //이동 목표
    private LayerMask stage; //지면 레이어마스크
    public float MoveSpeed = 7f; //이동 속도
    public float RollingDistance = 5f; //구르는 거리
    public float RollingTime = 0.35f; //구르는 시간


    public float StopTime; //경직 시간
    public float Barrier; //쉴드량
    public float NoDamageTime = 1f; //무적 시간

    private bool IsRolling = false; //구르기 여부
    private bool IsNoDamaged = false; //무적 여부

    [SerializeField]
    private GameObject AttackPrefab; //평타 프리팹
    [SerializeField]
    private GameObject qPrefab; //Q 스킬 프리팹
    [SerializeField]
    private GameObject wPrefab; //W 스킬 프리팹
    [SerializeField]
    private GameObject ePrefab; //E 스킬 프리팹
    [SerializeField]
    private GameObject rPrefab; //R 스킬 프리팹
    [SerializeField]
    private GameObject attackPosition; //공격 나가는 위치

    private Animator animator; 
    private InputManager inputManager;
    public PlayerState CurrentState; //현재 상태


    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
        inputManager = GameManager.Input;

        //키액션 구독 해지 (구독 중복 방지용)
        TakeControl();

        //키액션 구독
        BringBackControl();

        //스테이지 충돌 레이어 설정
        stage = LayerMask.GetMask("Stage");
    }
    IEnumerator NoDamage() //무적 부여 코루틴
    {
        //이미 무적 상태라면, 코루틴 종료
        if (IsNoDamaged) yield break;

        //무적 상태로 변경
        IsNoDamaged = true;

        //NoDamageTime만큼 무적 상태 유지
        yield return new WaitForSeconds(NoDamageTime);
        
        //무적 상태 해제
        IsNoDamaged = false;
    }


    public void Damaged(float damage) //받는 데미지 처리
    {
        //무적 상태라면 처리하지 않음
        if (IsNoDamaged) { return; }

        //Hp가 데미지만큼 줄어듬
        CurrentHp -= damage;

        //Hp 음수 방지 처리
        CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        //Hp가 0 이하라면
        if (CurrentHp <= 0f)
        {
            //죽음 처리 메서드 호출
            Die();
        }

    }

    public void Die() //죽음 처리
    {
        //현재 플레이어 상태를 Null로 변경
        CurrentState = PlayerState.Null;

        //키액션 구독 해지
        TakeControl();

        //데스카운트 감소
        GameManager.Instance.DeathCountDown();
        
        //Death 애니메이션 재생
        animator.Play("Death");
    }

    public void Stuned(float stunTime) //스턴 처리
    {
        //무적 상태라면 처리하지 않음
        if (IsNoDamaged) { return; }

        //현재 플레이어 상태를 Null로 변경
        CurrentState = PlayerState.Null;

        //키액션 구독 해지
        TakeControl();
        
        //Stun 애니메이션 재생
        animator.Play("Stun");

        //스턴 시간만큼 기다렸다가, 키액션 재구독
        Invoke("BringBackControl", stunTime);

    }

    public void Resurrection() //부활 처리
    {
        //무적 부여
        StartCoroutine(NoDamage());

        //키액션 재구독
        BringBackControl();

        //최대 Hp 초기화
        MaxHp = 100f;
        
        //최대 Mp 초기화
        MaxMp = 1000f;

        //현재 Hp 초기화
        CurrentHp = MaxHp;

        //현재 Mp 초기화
        CurrentMp = MaxMp;

        //현재 플레이어 상태를 Idle로 변경
        CurrentState = PlayerState.Idle;
    }

    void TakeControl() //캐릭터 조종 불가 처리
    {
        //키액션 구독 해지
        GameManager.Input.KeyAction -= OnMouse;
        GameManager.Input.KeyAction -= OnKeyboard;
    }

    void BringBackControl() //캐릭터 조종 가능 처리 
    {
        //키액션 구독
        GameManager.Input.KeyAction += OnMouse;
        GameManager.Input.KeyAction += OnKeyboard;

    }
    void Update()
    {
        //현재 플레이어 상태에 따른 애니메이션 등의 처리
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

    void HandleIdle() //Idle 상태 처리
    {
        //애니메이션 Idle 재생
        animator.Play("Idle");
    }

    void HandleMove() //Move 상태 처리
    {
        //애니메이션 Move 재생
        animator.Play("Move");

        //바라볼 방향 벡터 계산
        Vector3 direction = (targetPosition - transform.position).normalized;

        //목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        //플레이어가 바라볼 방향으로의 회전 처리
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, MoveSpeed / 2 * Time.deltaTime);
        }

        //플레이어가 목표 지점에 도착한다면
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            //현재 플레이어 상태를 Idle로 변경
            CurrentState = PlayerState.Idle;
        }
    }

    void HandleRoll() //Roll 상태 처리
    {
        //애니메이션 Roll 재생
        animator.Play("Roll");

        //목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (RollingDistance / RollingTime) * Time.deltaTime);

        //플레이어가 목표 지점에 도착한다면
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            //구르기 상태 해제
            IsRolling = false;

            //현재 플레이어 상태를 Idle로 변경
            CurrentState = PlayerState.Idle;
        }

    }

    void HandleAttack() //Attack 상태 처리
    {
        //현재 애니메이션 상태를 가져옴
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //현재 애니메이션 상태 이름이 Attack이면
        if (stateInfo.IsName("Attack"))
        {
            //애니메이션이 종료되면
            if (stateInfo.normalizedTime >= 1f)
            {
                //현재 플레이어 상태를 Idle로 변경
                CurrentState = PlayerState.Idle;
            }
        }
    }

    void HandleNull() //Null 상태 처리
    {
        //비어있음
    }

    void HandleUseQ() //UseQ 상태 처리 
    {
        //현재 애니메이션 상태를 가져옴
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //현재 애니메이션 상태 이름이 Q라면
        if (stateInfo.IsName("Q"))
        {
            //애니메이션이 종료되면
            if (stateInfo.normalizedTime >= 1f)
            {
                //키액션 구독
                BringBackControl();

                //현재 플레이어 상태를 Idle로 변경
                CurrentState = PlayerState.Idle;
            }
        }
    }
    
    void HandleUseW() //UseW 상태 처리
    {

    }

    void HandleUseE() //UseE 상태 처리
    {

    }

    void HandleUseR() //UseR 상태 처리
    {
        
    }

    void OnMouse() //마우스 입력 처리
    {
        //[Move]
        //좌클릭 했을 때, 구르기 상태가 아니면
        if (Input.GetMouseButton(1) && !IsRolling)
        {
            //마우스 위치 구하기
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //레이가 stage 레이어 오브젝트에 충돌했다면
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {
                //레이가 충돌한 오브젝트 태그가 Stage라면
                if (hit.collider.tag == "Stage")
                {
                    //목표 위치를 충돌 지점으로 설정
                    targetPosition = hit.point;
                    
                    //목표 위치를 바라봄
                    transform.LookAt(targetPosition);

                    //현재 플레이어 상태를 Move로 변경
                    CurrentState = PlayerState.Move;
                }
            }
        }

        //[Attack]
        //우클릭 했을 때, 평타 사용 가능이면
        if (Input.GetMouseButtonDown(0) && AutoAttack.Instance.IsUsable)
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //현재 플레이어 상태를 Attack으로 변경
                CurrentState = PlayerState.Attack;

                //Attack 애니메이션 트리거 생성
                animator.SetTrigger("Attack");

                //마우스 위치 구하기
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //레이가 stage 레이어 오브젝트에 충돌했다면
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
                {
                    //목표 위치를 충돌 지점으로 설정
                    targetPosition = hit.point;

                    //목표 위치를 바라봄
                    transform.LookAt(targetPosition);

                    //평타 인스턴스 생성
                    GameObject autoAttack = Instantiate(AttackPrefab, attackPosition.transform);
                    
                    //1초 뒤에 평타 인스턴스 삭제
                    Destroy(autoAttack, 1f);
                }
            }

        }
    }

    void OnKeyboard() //키보드 입력 처리
    {
        //[Roll]
        if (Input.GetKey(KeyCode.Space)) 
        {
            //마우스 위치 구하기
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //레이가 stage 레이어 오브젝트에 충돌했다면
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {

                //레이가 충돌한 오브젝트 태그가 Stage라면
                if (hit.collider.tag == "Stage")
                {
                    //목표 위치를 충돌 지점으로 설정
                    targetPosition = hit.point;

                    //바라볼 위치 지정
                    Vector3 lookDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
                    
                    //지정한 위치를 바라봄
                    transform.LookAt(lookDirection);

                    //구르기 상태 활성화
                    IsRolling = true;

                    //무적 부여
                    StartCoroutine(NoDamage());

                    //현재 플레이어 상태를 Roll로 변경
                    CurrentState = PlayerState.Roll;
                }
            }
        }

        //[Use Q]
        if (Input.GetKey(KeyCode.Q))
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //Q 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (Q.Instance.IsUsable && CurrentMp >= Q.Instance.UseMp)
                {
                    //Q 스킬 사용
                    Q.Instance.UseSkill();

                    //현재 플레이어 상태를 UseQ로 변경
                    CurrentState = PlayerState.UseQ;

                    //Q 애니메이션 트리거 설정 
                    animator.SetTrigger("Q");

                    //Q 인스턴스 생성
                    GameObject q = Instantiate(qPrefab, attackPosition.transform);
                    
                    //키액션 구독 해지
                    TakeControl();

                    //Q 인스턴스 2초 뒤 삭제
                    Destroy(q, 2f);
                }

                //Q 스킬 사용 불가 상태라면
                else if (!Q.Instance.IsUsable)
                {
                    Debug.Log("아직 스킬을 쓸 수 없다는 메시지 띄우기");
                }
               
                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < Q.Instance.UseMp)
                {
                    Debug.Log("MP 부족 메시지 띄우기");
                }
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            W.Instance.UseSkill(); //마나 줄어들고 쿨 돌아가기만 하는 중
            //바라본 방향으로 스킬을 설치, 범위 내 모든 적에게 대미지 부여
        }

        if (Input.GetKey(KeyCode.E))
        {
            E.Instance.UseSkill();
            //4초간 지속되는 실드 씌우기, 실드는 Hp보다 우선해서 까지니까 Damaged에서도 작업해줘야함
        }

        if (Input.GetKey(KeyCode.R))
        {
            R.Instance.UseSkill();
            //스킬 범위 내 모든 적에게 대미지 부여, 시전 시간동안 무적 

        }
    }
}




using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    public float RollingDistance = 8.5f; //구르는 거리
    public float RollingTime = 0.617f; //구르는 시간


    public float StopTime; //경직 시간
    public float Barrier; //쉴드량
    public float NoDamageTime = 1f; //무적 시간
    private float movementSpeedRatio = 0;


    private bool IsRolling = false; //구르기 여부
    private bool IsNoDamaged = false; //무적 여부

    public Image HpBar;
    public Image MpBar;
    public TextMeshProUGUI HpText; //Hp바 텍스트
    public TextMeshProUGUI MpText; //Mp바 텍스트

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
        HpText.text = CurrentHp + "/" + MaxHp;
        MpText.text = CurrentMp + "/" + MaxMp;


        //키액션 구독 해지 (구독 중복 방지용)
        TakeControl();

        //키액션 구독
        BringBackControl();

        //스테이지 충돌 레이어 설정
        stage = LayerMask.GetMask("Stage");

    }

    IEnumerator NoDamage(float noDamageTime) //무적 부여 코루틴
    {
        //이미 무적 상태라면, 코루틴 종료
        if (IsNoDamaged) yield break;

        //무적 상태로 변경
        IsNoDamaged = true;

        //NoDamageTime만큼 무적 상태 유지
        yield return new WaitForSeconds(noDamageTime);

        //무적 상태 해제
        IsNoDamaged = false;
    }

    public void BarrierSet()
    {
        Barrier = 50f;
        HpText.text = CurrentHp + "(+" + Barrier + ")" + "/" + MaxHp;
    }

    public void BarrierReset() //쉴드 리셋 (사용시, Invoke 통해서 호출하는 것으로 유지시간 설정)
    {
        Barrier = 0f;
        HpText.text = CurrentHp + "/" + MaxHp;
    }


    public void Damaged(float damage) //받는 데미지 처리
    {
        //무적 상태라면 처리하지 않음
        if (IsNoDamaged) { return; }
        else if (Barrier > 0f)
        {
            float lastDamage = damage - Barrier;

            Barrier -= damage;
            Barrier = Mathf.Clamp(Barrier, 0f, Mathf.Infinity);

            if (lastDamage >= 0f)
            {
                damage = lastDamage;

                //Hp가 데미지만큼 줄어듬
                CurrentHp -= damage;
                HpBar.fillAmount -= damage;
                HpText.text = CurrentHp + "/" + MaxHp;

                //Hp 음수 방지 처리
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp가 0 이하라면
                if (CurrentHp <= 0f)
                {
                    //죽음 처리 메서드 호출
                    Die();
                }
            }
            else
            {
                //Hp가 데미지만큼 줄어듬
                CurrentHp -= damage;
                HpBar.fillAmount = CurrentHp / MaxHp;

                //Hp 음수 방지 처리
                CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

                //Hp가 0 이하라면
                if (CurrentHp <= 0f)
                {
                    //죽음 처리 메서드 호출
                    Die();
                }
            }
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
        animator.SetTrigger("Death");
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
        animator.SetTrigger("Stun");

        //스턴 시간만큼 기다렸다가, 키액션 재구독
        Invoke("BringBackControl", stunTime);

    }

    public void Resurrection() //부활 처리
    {
        //무적 부여
        StartCoroutine(NoDamage(NoDamageTime));

        //키액션 재구독
        BringBackControl();

        //최대 Hp 초기화
        MaxHp = 100f;

        //최대 Mp 초기화
        MaxMp = 1000f;

        //현재 Hp 초기화
        CurrentHp = MaxHp;
        HpBar.fillAmount = CurrentHp / MaxHp;
        HpText.text = CurrentHp + "/" + MaxHp;

        //현재 Mp 초기화
        CurrentMp = MaxMp;
        MpBar.fillAmount = CurrentMp / MaxMp;
        MpText.text = CurrentMp + "/" + MaxMp;

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

        /*
         * 플레이어의 캐릭터 컨트롤 권한을 뺏는 일이 많아, 포션 섭취는 Update에서 처리합니다.
         */

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //(Hp포션 개수) > 0f 라면, Hp포션을 섭취
            //else: 포션을 사용할 수 없습니다.
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //(Mp포션 개수) > 0f 라면, Mp포션을 섭취
            //else: 포션을 사용할 수 없습니다.
        }

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
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 0, MoveSpeed * Time.deltaTime);
        animator.SetFloat("Move", movementSpeedRatio);
        //애니메이션 Idle 
        animator.Play("IdleMove");
    }

    void HandleMove() //Move 상태 처리
    {
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 1, MoveSpeed * Time.deltaTime);

        animator.SetFloat("Move",movementSpeedRatio);

        //애니메이션 Move 재생
        animator.Play("IdleMove");

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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Roll"))
        {
            animator.Play("Roll");
        }
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
                //키액션 구독
                BringBackControl();
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
        //현재 애니메이션 상태를 가져옴
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //현재 애니메이션 상태 이름이 W라면
        if (stateInfo.IsName("W"))
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

    void HandleUseE() //UseE 상태 처리
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

    void HandleUseR() //UseR 상태 처리
    {
        //현재 애니메이션 상태를 가져옴
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //현재 애니메이션 상태 이름이 R라면
        if (stateInfo.IsName("R"))
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

    void OnMouse() //마우스 입력 처리
    {
        //[Move]
        //우클릭 했을 때, 구르기 상태가 아니면
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
        //좌클릭 했을 때, 평타 사용 가능이면
        if (Input.GetMouseButtonDown(0) && AutoAttack.Instance.IsUsable)
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //키액션 구독 해제
                TakeControl();

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
                    Destroy(autoAttack, 0.85f);
                }
            }

        }
    }

    void OnKeyboard() //키보드 입력 처리
    {
        //[Roll]
        if (Input.GetKeyDown(KeyCode.Space) && !IsRolling)
        {
            //마우스 위치 구하기
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;          

            //레이가 stage 레이어 오브젝트에 충돌했다면
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {
                // 마우스 위치에서 현재 플레이어 위치까지의 방향 구하기
                Vector3 direction = (hit.point - transform.position).normalized;

                // 목표 위치를 현재 위치에서 RollingDistance만큼 이동한 지점으로 설정
                targetPosition = transform.position + direction * RollingDistance;

                // 바라볼 위치 지정 (y 값만 현재 플레이어의 y로 고정)
                Vector3 lookDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);


                //지정한 위치를 바라봄
                transform.LookAt(lookDirection);

                //구르기 상태 활성화
                IsRolling = true;

                //무적 부여
                StartCoroutine(NoDamage(NoDamageTime));

                //현재 플레이어 상태를 Roll로 변경
                CurrentState = PlayerState.Roll;

            }
        }

        //[Use Q]
        if (Input.GetKeyDown(KeyCode.Q))
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

        //[UseW]
        if (Input.GetKeyDown(KeyCode.W))
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //W 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (W.Instance.IsUsable && CurrentMp >= W.Instance.UseMp)
                {

                    //W 스킬 사용
                    W.Instance.UseSkill();

                    //현재 플레이어 상태를 UseW로 변경
                    CurrentState = PlayerState.UseW;

                    //Q 애니메이션 트리거 설정 
                    animator.SetTrigger("W");
                    
                    //키액션 구독 해지
                    TakeControl();

                    //스킬 시전 위치 설정
                    Vector3 wPosition = attackPosition.transform.position + attackPosition.transform.forward * 1f;
                    wPosition.y = 3f;

                    //W 스킬 설치
                    GameObject w = Instantiate(wPrefab, wPosition, transform.rotation);

                    //설치된 W 스킬 3초 뒤 삭제 (TEMP)
                    Destroy(w, 3f);



                }
                //W 스킬 사용 불가 상태라면
                else if (!W.Instance.IsUsable)
                {
                    Debug.Log("아직 스킬을 쓸 수 없다는 메시지 띄우기");
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < W.Instance.UseMp)
                {
                    Debug.Log("MP 부족 메시지 띄우기");
                }
            }
        }

        //[UseE]
        if (Input.GetKeyDown(KeyCode.E))
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //E 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (E.Instance.IsUsable && CurrentMp >= E.Instance.UseMp)
                {
                    //E 스킬 사용
                    E.Instance.UseSkill();

                    //현재 플레이어 상태를 UseR로 변경
                    CurrentState = PlayerState.UseE;

                    BarrierSet();

                    //4초 뒤 쉴드량 0으로 초기화
                    Invoke("BarrierReset",4f);

                    //E 애니메이션 트리거 설정 
                    animator.SetTrigger("E");

                    //E 인스턴스 생성
                    GameObject e = Instantiate(ePrefab, attackPosition.transform);

                    //E 인스턴스 4초 뒤 삭제 (TEMP)
                    Destroy(e, 4f);
                }

                //R 스킬 사용 불가 상태라면
                else if (!R.Instance.IsUsable)
                {
                    Debug.Log("아직 스킬을 쓸 수 없다는 메시지 띄우기");
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < R.Instance.UseMp)
                {
                    Debug.Log("MP 부족 메시지 띄우기");
                }
            }

        }

        //[UseR]
        if (Input.GetKeyDown(KeyCode.R))
        {
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //R 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (R.Instance.IsUsable && CurrentMp >= R.Instance.UseMp)
                {
                    //R 스킬 사용
                    R.Instance.UseSkill();

                    //현재 플레이어 상태를 UseR로 변경
                    CurrentState = PlayerState.UseR;

                    //4초간 무적 부여 (TEMP)
                    NoDamage(4f);

                    //R 애니메이션 트리거 설정 
                    animator.SetTrigger("R");

                    //R 인스턴스 생성
                    GameObject r = Instantiate(rPrefab, attackPosition.transform);

                    //키액션 구독 해지
                    TakeControl();

                    //R 인스턴스 4초 뒤 삭제 (TEMP)
                    Destroy(r, 4f);
                }

                //R 스킬 사용 불가 상태라면
                else if (!R.Instance.IsUsable)
                {
                    Debug.Log("아직 스킬을 쓸 수 없다는 메시지 띄우기");
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < R.Instance.UseMp)
                {
                    Debug.Log("MP 부족 메시지 띄우기");
                }
            }
        }
    }
}





using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Playables;
using static Player;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using JetBrains.Annotations;
using Unity.AppUI.UI;
using static UnityEngine.GridBrushBase;
using System;

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
    public Hp PlayerHp;

    public float MaxMp = 1000; //최대MP
    public float CurrentMp = 1000; //현재MP
    public float ManaExcel = 1; // MP 가속

    public float Power = 100; //공격력
    public float AddedPower = 0; //추가 공격력

    public float PlayerBarrier = 150;

    public bool IsDrainHp = false;
    public bool OneLife = false;

    private Vector3 targetPosition; //이동 목표
    private LayerMask stage; //지면 레이어마스크
    public float MoveSpeed = 7f; //이동 속도
    public float RollingDistance = 8.5f; //구르는 거리
    public float RollingTime = 0.617f; //구르는 시간


    public float StopTime; //경직 시간
    private float movementSpeedRatio = 0;


    private bool IsRolling = false; //구르기 여부
    public float RollingCoolTime; //구르기 쿨타임
    public Image RollingCool; //구르기 쿨타임 UI
    public TextMeshProUGUI RollCoolText; //구르기 쿨타임 시간UI

    public Image MpBar;

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
    [SerializeField]
    private GameObject mpHealEffect; 

    public Animator animator;
    private InputManager inputManager;
    public PlayerState CurrentState; //현재 상태


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        StartCoroutine(MpNaturalRestoration());
        animator = GetComponent<Animator>();
        inputManager = GameManager.Input;
        PlayerHp = GetComponent<Hp>();
        MpText.text = CurrentMp + "/" + MaxMp;


        //구르기 쿨타임 초기화
        RollingCoolTime = 0f;

        //구르기 시간UI 초기화
        RollCoolText.text = "";

        //구르기 쿨 UI 초기화
        RollingCool.fillAmount = 0;

        //키액션 구독 해지 (구독 중복 방지용)
        TakeControl();

        //키액션 구독
        BringBackControl();

        //스테이지 충돌 레이어 설정
        stage = LayerMask.GetMask("Stage");
    }

    IEnumerator MpNaturalRestoration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (CurrentMp < MaxMp)
            {
                CurrentMp += 10f * ManaExcel;
                CurrentMp = Mathf.Clamp(CurrentMp, 0f, MaxMp);
                MpBar.fillAmount = CurrentMp / MaxMp;
                MpText.text = CurrentMp + "/" + MaxMp;
            }
        }
        

    }

    public void Stuned(float stunTime) //스턴 처리
    {
        //무적 상태라면 처리하지 않음
        if (PlayerHp.IsNoDamaged) { return; }

        //현재 플레이어 상태를 Null로 변경
        CurrentState = PlayerState.Null;

        //키액션 구독 해지
        TakeControl();

        //Stun 애니메이션 재생
        animator.SetTrigger("Stun");

        //스턴 시간만큼 기다렸다가, 키액션 재구독
        Invoke("BringBackControl", stunTime);

    }

    public void MpHeal(float heal)
    {
        CurrentMp += heal;
        CurrentMp = Mathf.Clamp(CurrentMp, 0f, MaxMp);
        MpBar.fillAmount = CurrentMp/MaxMp;
        MpText.text = CurrentMp + "/" + MaxMp;
        if (mpHealEffect != null)
        {
            mpHealEffect.SetActive(true);
            Invoke("MpHealEffectEnd", 0.55f);
        }

    }
    void MpHealEffectEnd() { mpHealEffect.SetActive(false); }

    public void TakeControl() //캐릭터 조종 불가 처리
    {
        //키액션 구독 해지
        GameManager.Input.KeyAction -= OnMouse;
        GameManager.Input.KeyAction -= OnKeyboard;
    }

    public void BringBackControl() //캐릭터 조종 가능 처리 
    {
        //키액션 구독
        GameManager.Input.KeyAction += OnMouse;
        GameManager.Input.KeyAction += OnKeyboard;

    }
    
    void Update()
    {
        if (RollingCoolTime > 0)
        {
            //시간 따라 쿨타임 돔
            RollingCoolTime -= Time.deltaTime;

            //쿨타임 음수 방지 처리
            RollingCoolTime = Mathf.Clamp(RollingCoolTime, 0, Mathf.Infinity);

            if (RollingCoolTime >= 1f)
            {
                RollCoolText.text = ((int)RollingCoolTime).ToString();
            }
            else
            {
                RollCoolText.text = Math.Round(RollingCoolTime, 1).ToString();
            }

            RollingCool.fillAmount = RollingCoolTime / 3;
        }
        else if (RollingCoolTime <= 0)
        {
            //구르기 시간UI 초기화
            RollCoolText.text = "";

            //구르기 쿨 UI 초기화
            RollingCool.fillAmount = 0;
        }

        /*
         * 플레이어의 캐릭터 컨트롤 권한을 뺏는 일이 많아, 포션 섭취는 Update에서 처리합니다.
         */

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
            if (CurrentState != PlayerState.Null && Potion.Instance.HpPotion > 0f)
            {
                //사운드 실행 구문 
                AudioMixerController.Instance.PlayerStartClip(DefineMusicName.Potion);
                PlayerHp.HpHeal(Potion.Instance.HpHeal);
                Potion.Instance.HpPotion--;
                Potion.Instance.HpPotionText.text = (Potion.Instance.HpPotion).ToString();
            }
            else
            {
                WarningMessage.Instance.NoPotion();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            
            if (CurrentState != PlayerState.Null && Potion.Instance.MpPotion > 0f)
            {
                //사운드 실행 구문 
                AudioMixerController.Instance.PlayerStartClip(DefineMusicName.Potion);
                MpHeal(Potion.Instance.MpHeal);
                Potion.Instance.MpPotion--;
                Potion.Instance.MpPotionText.text = (Potion.Instance.MpPotion).ToString();
            }
            else
            {
                WarningMessage.Instance.NoPotion();
            }
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

        animator.SetFloat("Move", movementSpeedRatio);

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

        if (stateInfo.IsName("E"))
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
                    transform.LookAt(new Vector3(targetPosition.x,transform.position.y,targetPosition.z));

                    //현재 플레이어 상태를 Move로 변경
                    CurrentState = PlayerState.Move;
                }
            }
        }

        //[Attack]
        //좌클릭 했을 때, 평타 사용 가능이면
        if (Input.GetMouseButtonDown(0) && AutoAttack.IsUsable)
        {
            AudioMixerController.Instance.PlayerStartClip(DefineMusicName.AutoAttack);
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
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Space]))
        {
         
            
            if(!IsRolling && RollingCoolTime <= 0)
            {
                
                //사운드 실행 구문
                AudioMixerController.Instance.PlayerStartClip(DefineMusicName.Rolling);
                
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

                    //구르기 쿨타임 적용
                    RollingCoolTime = 3f;

                    //구르기 쿨 시간 UI에 쿨타임 표시
                    RollCoolText.text = ((int)RollingCoolTime).ToString();

                    //구르기 쿨 UI 활성화
                    RollingCool.fillAmount = 1;

                    //무적 부여
                    StartCoroutine(PlayerHp.NoDamage(RollingTime));

                    //현재 플레이어 상태를 Roll로 변경
                    CurrentState = PlayerState.Roll;
                }
            }
            else if (RollingCoolTime > 0f)
            {
                WarningMessage.Instance.SkillCooling();
            }
        }

        //[Use Q]
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Q]))
        {
            
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                
                //Q 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (Q.Instance.IsUsable && CurrentMp >= Q.Instance.UseMp)
                {
                    //사운드 실행 구문
                    AudioMixerController.Instance.PlayerStartClip(DefineMusicName.Q);
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
                    WarningMessage.Instance.SkillCooling();
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < Q.Instance.UseMp)
                {
                    WarningMessage.Instance.NoMp();
                }
            }
        }

        //[UseW]
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.W]))
        {
        
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //W 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (W.Instance.IsUsable && CurrentMp >= W.Instance.UseMp)
                {
                    //사운드 실행 구문
                    AudioMixerController.Instance.PlayerStartClip(DefineMusicName.W);
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
                    WarningMessage.Instance.SkillCooling();
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < W.Instance.UseMp)
                {
                    WarningMessage.Instance.NoMp();
                }
            }
        }

        //[UseE]
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.E]))
        {
         
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //E 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (E.Instance.IsUsable && CurrentMp >= E.Instance.UseMp)
                {
                    //사운드 실행 구문
                    AudioMixerController.Instance.PlayerStartClip(DefineMusicName.E);
                    //E 스킬 사용
                    E.Instance.UseSkill();

                    //현재 플레이어 상태를 UseR로 변경
                    CurrentState = PlayerState.UseE;

                    PlayerHp.BarrierSet(PlayerBarrier);

                    //4초 뒤 쉴드량 0으로 초기화
                    PlayerHp.Invoke("BarrierReset", 4f);

                    //E 애니메이션 트리거 설정 
                    animator.SetTrigger("E");

                    //E 인스턴스 생성
                    GameObject e = Instantiate(ePrefab, attackPosition.transform);

                    //E 인스턴스 4초 뒤 삭제 (TEMP)
                    Destroy(e, 4f);

                    StartCoroutine(MonitorEInstance(e));
                }

                //E 스킬 사용 불가 상태라면
                else if (!E.Instance.IsUsable)
                {
                    WarningMessage.Instance.SkillCooling();
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < E.Instance.UseMp)
                {
                    WarningMessage.Instance.NoMp();
                }
            }

        }

        //[UseR]
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.R]))
        {
         
            //현재 플레이어 상태가 Idle 또는 Move라면
            if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Move)
            {
                //R 사용 가능이며, 현재 Mp가 스킬 소모 Mp 이상이라면
                if (R.Instance.IsUsable && CurrentMp >= R.Instance.UseMp)
                {
                    //사운드 실행 구문
                    AudioMixerController.Instance.PlayerStartClip(DefineMusicName.R);
                    //R 스킬 사용
                    R.Instance.UseSkill();

                    //현재 플레이어 상태를 UseR로 변경
                    CurrentState = PlayerState.UseR;

                    //4초간 무적 부여 (TEMP)
                    StartCoroutine(PlayerHp.NoDamage(4f));

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
                    WarningMessage.Instance.SkillCooling();
                }

                //현재 Mp가 스킬 소모 Mp 미만이라면
                else if (CurrentMp < R.Instance.UseMp)
                {
                    WarningMessage.Instance.NoMp();
                }
            }
        }
    }

    public void DrainHp()
    {
        if(IsDrainHp)
        {
            GetComponent<Hp>().HpHeal(1f);
        }
    }

    IEnumerator MonitorEInstance(GameObject eInstance)
    {
        Debug.Log("MonitorEInstance 코루틴 시작!");

        while (eInstance != null)
        {

            if (PlayerHp.Barrier <= 0)
            {
                Destroy(eInstance);
                yield break;
            }

            yield return null;
        }
    }

}

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BehaviourAI : MonoBehaviour
{
    enum BossState
    {
        Locomotion,
        Attack,
    }

    public GameObject TargetPlayer;

    [SerializeField] float movementSpeed;
    [SerializeField] float searchBoundary; // 탐색 반경
    [SerializeField] BossState bossState;

    [SerializeField] int testSkillStart;
    [SerializeField] int testSkillEnd;

    BehaviourTreeRunner locomotionBT; // For Locomotion
    BehaviourTreeRunner attackBT; // For Attack
    Animator animator;

    float movementSpeedRatio;
    int patternNumber;
    bool animFinish;
    bool switchingClip;

    void Start()
    {
        locomotionBT = new BehaviourTreeRunner(SettingFollowBT());
        attackBT = new BehaviourTreeRunner(SettingAttackBT());

        animator = GetComponent<Animator>();

        patternNumber = Random.Range(testSkillStart, testSkillEnd);
        animFinish = false;
        switchingClip = false;
    }

    void Update()
    {
        if (bossState == BossState.Locomotion)
            locomotionBT.Operate();
        else if (bossState == BossState.Attack)
            attackBT.Operate();
    }

    // Init LocomotionBT
    // 가까우면 AttackBT로 바꾸고 아니면 플레이어를 향해 쫓아감.
    INode SettingFollowBT()
    {
        INode root = new SelectorNode(
            new List<INode>()
            {
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(IsNearPlayer),
                        new ActionNode(Wait),
                        new ActionNode(OperateAttackBT)
                    }
                ),
                new ActionNode(FollowPlayer)
            }
        );

        return root;
    }

    // Init AttackBT
    // 0 ~ 3 번 중 하나의 공격을 한 후에 공격 에니메이션이 끝났다면 다음 공격을 준비.
    // 4번 이후는 추가 또는 삭제 예정
    // Attack 종류에 따른 탐색반경 추가 예정
    INode SettingAttackBT()
    {
        INode root = new SelectorNode(
            new List<INode>()
            {
                new SelectorNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckWithLog),
                        new SelectorNode(
                            new List<INode>()
                            {
                                new ActionNode(Attack0),
                                new ActionNode(Attack1),
                                new ActionNode(Attack2),
                                new ActionNode(Attack3),
                                new ActionNode(Attack4),
                                new ActionNode(Attack5)
                            }
                        ),
                        new ActionNode(WaitAnimFinish),
                        new ActionNode(SetNextAttack)
                    }
                )
            }
        );

        return root;
    }

    NodeState CheckWithLog()
    {

        return NodeState.Failure;
    }

    // 가까이 있는지 체크하는 노드
    NodeState IsNearPlayer()
    {
        if (Vector3.Magnitude(TargetPlayer.transform.position - transform.position) < searchBoundary)
        {
            return NodeState.Success;
        }
        return NodeState.Failure;
    }

    // AttackBT 로 전환
    NodeState OperateAttackBT()
    {
        switchingClip = false;
        bossState = BossState.Attack;

        return NodeState.Success;
    }

    // 패턴을 사용한 후에 기다리는 코드로 변경 예정
    NodeState Wait()
    {
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 0, movementSpeed * Time.deltaTime);
        animator.SetFloat("MovementSpeed", movementSpeedRatio);
        return NodeState.Success;
    }

    // 플레이어를 따라가는 함수
    NodeState FollowPlayer()
    {
        if (Vector3.Magnitude(TargetPlayer.transform.position - transform.position) < searchBoundary)
        {
            return NodeState.Success;
        }

        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 1, movementSpeed * Time.deltaTime);
        animator.SetFloat("MovementSpeed", movementSpeedRatio);
        transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, Time.deltaTime * movementSpeed);
        transform.LookAt(TargetPlayer.transform);
        return NodeState.Running;
    }

    // 애니메이션이 끝났는지 체크 >> OnAnimationFinished 함수가 각 애니메이션이 붙어있음
    NodeState WaitAnimFinish()
    {
        if (!animFinish) return NodeState.Success;
        return NodeState.Failure;
    }

    // 다음 스킬 사용 번호를 미리 정하고 Locomotion으로 변경
    NodeState SetNextAttack()
    {
        patternNumber = Random.Range(testSkillStart, testSkillEnd);

        bossState = BossState.Locomotion;

        animator.SetTrigger("IdleWalk");

        return NodeState.Success;
    }

    // 장판이 플레이어 바닥에 생기는 기술
    NodeState Attack0()
    {
        if (patternNumber == 0)
        {
            if (!switchingClip)
            {
                switchingClip = true;
                transform.LookAt(TargetPlayer.transform);
                animator.SetTrigger("TargetSpell");
            }
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    // 칼날을 날림
    NodeState Attack1()
    {
        if (patternNumber == 1)
        {
            if (!switchingClip)
            {
                switchingClip = true;
                transform.LookAt(TargetPlayer.transform);
                animator.SetTrigger("GroundScratching");
            }
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    // 몬스터 위치에서 폭발하는 스킬
    NodeState Attack2()
    {
        if (patternNumber == 2)
        {
            if (!switchingClip)
            {
                switchingClip = true;
                transform.LookAt(TargetPlayer.transform);
                animator.SetTrigger("Explosion");
            }
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    // 랜덤 위치에 폭발
    NodeState Attack3()
    {
        if (patternNumber == 3)
        {
            if (!switchingClip)
            {
                switchingClip = true;
                transform.LookAt(TargetPlayer.transform);
                animator.SetTrigger("BloodBoom");
            }
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    NodeState Attack4()
    {
        if (patternNumber == 4)
        {
            if (!switchingClip)
            {
                switchingClip = true;
                transform.LookAt(TargetPlayer.transform);
                animator.SetTrigger("RightHand");
            }
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    NodeState Attack5()
    {
        if (patternNumber == 5)
        {
            Debug.Log("Attack5");
            return NodeState.Success;
        }

        return NodeState.Failure;
    }

    // 애니메이션이 끝나는 순간 애니메이션 컨트롤러의 웨이트를 바꿔주기 위한 함수
    public void OnAnimationFinished()
    {
        animFinish = true;
        switchingClip = false;
        patternNumber = -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchBoundary);
    }
}

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
    INode SettingAttackBT()
    {
        INode root = new SelectorNode(
            new List<INode>()
            {
                new SelectorNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckWithLog),
                        // 애니메이션을 관리할 떄 이미 patternNumber를 활용해서 애니메이션을 재생한다면?
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


    NodeState OperateAttackBT()
    {
        switchingClip = false;
        bossState = BossState.Attack;

        return NodeState.Success;
    }

    NodeState Wait()
    {
        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 0, movementSpeed * Time.deltaTime);
        animator.SetFloat("MovementSpeed", movementSpeedRatio);
        return NodeState.Success;
    }

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

    NodeState WaitAnimFinish()
    {
        if (!animFinish) return NodeState.Success;
        return NodeState.Failure;
    }

    NodeState SetNextAttack()
    {
        patternNumber = Random.Range(testSkillStart, testSkillEnd);

        bossState = BossState.Locomotion;

        animator.SetTrigger("IdleWalk");

        return NodeState.Success;
    }

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

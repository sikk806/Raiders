//using System.Collections;
//using System.Drawing;
//using UnityEngine;
//using UnityEngine.TextCore.Text;
//using UnityEngine.UIElements;

//public enum PlayerState
//{
//    Idle,
//    Move,
//    Roll,
//    Attack,
//}

//public class PlayerController : MonoBehaviour
//{
//    public float MaxSpeed = 7f;
//    public float RollingLength = 5f;
//    public float RollingTime = 0.617f;

//    new private Rigidbody rigidbody;

//    private Animator animator;
//    private Vector3 targetPosition;
//    private float movementSpeedRatio;
//    private float comboTimer = 0f;
//    private bool comboTrigger = false;
//    private int comboCount = 0;

//    LayerMask stageLayer;
//    public PlayerState playerState;
//    PlayerState beforeState;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        rigidbody = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();

//        beforeState = PlayerState.Idle;
//        playerState = PlayerState.Idle;
//        stageLayer = LayerMask.GetMask("Stage");

//        GameManager.Input.KeyAction -= OnMouseEvent;
//        GameManager.Input.KeyAction += OnMouseEvent;

//        GameManager.Input.KeyAction -= OnKeyEvent;
//        GameManager.Input.KeyAction += OnKeyEvent;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        switch (playerState)
//        {
//            case PlayerState.Idle:
//                UpdateIdle();
//                break;
//            case PlayerState.Move:
//                UpdateMoving();
//                break;
//            case PlayerState.Roll:
//                UpdateRolling();
//                break;
//        }
//    }

//    void UpdateIdle()
//    {
//        movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 0, MaxSpeed * Time.deltaTime);
//        animator.SetFloat("MovementSpeed", movementSpeedRatio);
//        animator.Play("IdleRun");
//    }

//    void UpdateMoving()
//    {
//        if (playerState == PlayerState.Move)
//        {
//            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MaxSpeed * Time.deltaTime);
//            movementSpeedRatio = Mathf.Lerp(movementSpeedRatio, 1, MaxSpeed * Time.deltaTime);
//            animator.SetFloat("MovementSpeed", movementSpeedRatio);
//            animator.Play("IdleRun");

//            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
//            {
//                playerState = PlayerState.Idle;
//            }
//        }
//    }

//    void UpdateRolling()
//    {
//        if (playerState == PlayerState.Roll)
//        {
//            float rollSpeed = RollingLength / RollingTime;
//            transform.position = Vector3.MoveTowards(transform.position, targetPosition, rollSpeed * Time.deltaTime);

//            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
//            {
//                playerState = PlayerState.Idle;
//            }
//        }
//    }

//    void Attacking()
//    {

//    }

//    public void EndAttacking()
//    {
//        if (!comboTrigger)
//        {
//            comboCount = 0;
//            animator.SetInteger("ComboCount", comboCount);

//            comboTrigger = false;
//            animator.SetTrigger("SetLocomotion");
//            playerState = PlayerState.Idle;
//        }
//        else
//        {
//            comboCount++;
//            animator.SetInteger("ComboCount", comboCount);
//            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
//            Vector2 mousePosition = Input.mousePosition;

//            Vector2 direction = (mousePosition - screenCenter).normalized;

//            targetPosition = transform.position - new Vector3(direction.x, 0, direction.y) * RollingLength;
//            transform.LookAt(targetPosition);
//            comboTrigger = false;
//        }
//    }

//    void OnMouseEvent()
//    {
//        if (Input.GetMouseButton(1) && (playerState == PlayerState.Idle || playerState == PlayerState.Move))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            float maxDistance = 100f;

//            if (Physics.Raycast(ray, out hit, maxDistance, stageLayer))
//            {
//                if (hit.collider.tag == "Stage")
//                {
//                    targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
//                    transform.LookAt(targetPosition);
//                    playerState = PlayerState.Move;
//                }
//            }
//        }

//        if (Input.GetMouseButtonDown(0))
//        {
//            if (playerState == PlayerState.Idle || playerState == PlayerState.Move)
//            {
//                playerState = PlayerState.Attack;
//                animator.SetTrigger("SetAttack");
//                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
//                Vector2 mousePosition = Input.mousePosition;

//                Vector2 direction = (mousePosition - screenCenter).normalized;

//                targetPosition = transform.position - new Vector3(direction.x, 0, direction.y) * RollingLength;
//                transform.LookAt(targetPosition);
//            }
//            else if (playerState == PlayerState.Attack)
//            {
//                float animRate = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//                if (animRate < 0.8f)
//                {
//                    comboTrigger = true;
//                }
//                else
//                {
//                    comboCount = 0;
//                    animator.SetInteger("ComboCount", comboCount);

//                    comboTrigger = false;
//                    animator.SetTrigger("SetLocomotion");
//                    playerState = PlayerState.Idle;
//                }
//                // 애니메이션 시간 체크 후 일정 시간 이전에 클릭을 했다면 다음 콤보로 이어지도록.
//                // 애니메이션 시간 체크 후 일정 시간 이후에 클릭을 했다면 Locomotion으로 이어지도록.
//            }
//        }
//    }

//    void OnKeyEvent()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
//            Vector2 mousePosition = Input.mousePosition;

//            Vector2 direction = (mousePosition - screenCenter).normalized;

//            targetPosition = transform.position - new Vector3(direction.x, 0, direction.y) * RollingLength;
//            transform.LookAt(targetPosition);

//            PlayerState beforeState = playerState;
//            playerState = PlayerState.Roll;

//            animator.CrossFade("Roll", 0.05f);
//        }
//    }

//    void OnEventReserve()
//    {

//    }
//}

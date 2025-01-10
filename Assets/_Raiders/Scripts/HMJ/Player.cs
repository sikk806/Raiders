using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using static Player;

public enum State
{
    Idle,
    Move,
    Roll
}

public class Player : MonoBehaviour
{
    public float MaxHp;
    public float CurrentHp;
    public float MaxMP;
    public float CurrentMP;

    public float Power;
    public float WeaponPower;

    private Vector3 targetPosition;
    private LayerMask stage;
    public float MoveSpeed = 7f;
    public float RollingDistance;
    public float RollingTime;


    public float StopTime;
    public float Barrier;
    public float NoDamageTime;


    private Animator animator;
    private InputManager inputManager;
    public State CurrentState;
    void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Move:
                HandleMove();
                break;
            case State.Roll:
                HandleRoll();
                break;
        }
    }

    void HandleIdle()
    {
        animator.Play("Idle");
    }

    void HandleMove()
    {
        animator.Play("Move");
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, MoveSpeed / 2 * Time.deltaTime);
        }
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            CurrentState = State.Idle;
        }
    }


    void HandleRoll()
    {

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GameManager.Input;

        GameManager.Input.KeyAction -= OnMouse;
        GameManager.Input.KeyAction += OnMouse;

        GameManager.Input.KeyAction -= OnKeyboard;
        GameManager.Input.KeyAction += OnKeyboard;

        stage = LayerMask.GetMask("Stage");
    }


    void OnMouse()
    {
        if (Input.GetMouseButton(1)) //Move
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stage))
            {
                if (hit.collider.tag == "Stage")
                {
                    targetPosition = hit.point;
                    transform.LookAt(targetPosition);
                    CurrentState = State.Move;
                }
            }



            if (Input.GetMouseButton(0)) //Attack
            {

            }

        }


    }
    void OnKeyboard()
    {
        if (Input.GetKey(KeyCode.Space)) //Roll
        {
            //animator.SetTrigger("Roll");
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, RollingDistance/RollingTime * Time.deltaTime);

        }
    }
}




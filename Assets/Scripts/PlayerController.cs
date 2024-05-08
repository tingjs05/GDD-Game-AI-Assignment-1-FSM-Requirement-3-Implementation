using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    enum State 
    {
        IDLE, 
        MOVING, 
        PUSHING, 
        STUNNED, 
        DEATH
    }

    State currentState;
    Rigidbody rb;
    Coroutine coroutine;
    float timeElapsed = 0f;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float pushDuration = 2f;
    [SerializeField] float stunDuration = 2.5f;

    public Vector3 MoveDir { get; private set; }

    public static event System.Action<PlayerController> PushedObject;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = State.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
                idle();
                break;
            case State.MOVING:
                moving();
                break;
            case State.PUSHING:
                pushing();
                break;
            case State.STUNNED:
                if (coroutine != null) return;
                coroutine = StartCoroutine(stunned());
                break;
            case State.DEATH:
                death();
                break;
            default:
                Debug.LogError("State not found.");
                break;

        }
        
        // reset move direction
        MoveDir = Vector3.zero;
    }

    // handle states
    void idle()
    {
        if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) == Vector2.zero) return;
        currentState = State.MOVING;
    }

    void moving()
    {
        MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        
        // check for state transition to idle
        if (MoveDir == Vector3.zero)
        {
            currentState = State.IDLE;
            return;
        }

        // check for running (shift pressed), if running, multiple speed by 1.5f.
        rb.AddForce(MoveDir * moveSpeed * (Input.GetKey(KeyCode.LeftShift)? 1.5f : 1f));
    }

    void pushing()
    {
        if (timeElapsed >= pushDuration)
        {
            PushedObject?.Invoke(this);
            currentState = State.IDLE;
            return;
        }

        if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) != Vector2.zero)
        {
            timeElapsed = 0f;
            currentState = State.MOVING;
            return;
        }

        timeElapsed += Time.deltaTime;
    }

    void death()
    {
        Debug.Log("Player Died: Game Lost!");
        Destroy(gameObject);
    }

    IEnumerator stunned()
    {
        yield return new WaitForSeconds(stunDuration);
        coroutine = null;
        currentState = State.IDLE;
    }
}

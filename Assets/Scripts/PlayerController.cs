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

    [SerializeField] float moveSpeed = 5f;

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
                stunned();
                break;
            case State.DEATH:
                death();
                break;
            default:
                Debug.LogError("State not found.");
                break;

        }
    }

    // handle states
    void idle()
    {
        if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) == Vector2.zero) return;
        currentState = State.MOVING;
    }

    void moving()
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        
        // check for state transition to idle
        if (moveDir == Vector3.zero)
        {
            currentState = State.IDLE;
            return;
        }

        // check for running (shift pressed), if running, multiple speed by 1.5f.
        rb.AddForce(moveDir * moveSpeed * (Input.GetKey(KeyCode.LeftShift)? 1.5f : 1f));
    }

    void pushing()
    {

    }

    void stunned()
    {

    }

    void death()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BashPhase
{
    None,
    Start,
    Active,
    End
}

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    public float speed = 10f;
    public float jumpHeight = 10f;
    public float bashTime;
    public bool canBash = false;
    public BashPhase bashPhase = 0;
    public BashableObject bashObject;

    private float move;
    private float bash;
    private float jump;
    private Vector2 bashMove;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded())
        {
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(move * speed * Time.deltaTime, jumpHeight * jump * Time.deltaTime, 0), ForceMode.Impulse);
        }
        else if(!isGrounded())
        {
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(move * speed * Time.deltaTime, 0, 0), ForceMode.Impulse);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<float>();
    }

    public void Bash(InputAction.CallbackContext context)
    {
        if(canBash)
        {
            bash = context.ReadValue<float>();
        }
    }

    public void BashMove(InputAction.CallbackContext context)
    {
        if(bashPhase == BashPhase.Active)
        {
            bashMove = context.ReadValue<Vector2>();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(bashPhase == BashPhase.None)
        {
            jump = context.ReadValue<float>();
        }
    }

    private IEnumerator BashCooldown()
    {
        yield return null;
    }

    private bool isGrounded()
    {
        bool grounded = false;

        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y / 2 + 0.5f))
        {
            grounded = true;
        }

        return grounded;
    }



}

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
    private Vector2 mousePos;

    private float time;
    private float lerpDuration = 0.5f;


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

        if(move == 0)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
        }

        if(bash == 1)
        {
            StartBash();
        }
        else if(bash == 0 && bashPhase != BashPhase.None || bashPhase != BashPhase.End)
        {
            bashPhase = BashPhase.End;
            EndBash();
        }

    }

    private void Update()
    {
        BashMove();

        if(bashPhase == BashPhase.Start)
        {
            if(time < lerpDuration)
            {
                Time.timeScale = Mathf.Lerp(1, 0, time/lerpDuration);
                time += Time.deltaTime;
            }
            else
            {
                Time.timeScale = 0;
                DuringBash();
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(bashPhase == 0)
        {
            move = context.ReadValue<float>();
        }
    }

    public void Bash(InputAction.CallbackContext context)
    {
        if (canBash && bashObject != null)
        {
            bash = context.ReadValue<float>();
        }
    }

    private void BashMove()
    {
        if(bashPhase == BashPhase.Active)
        {
            mousePos = Input.mousePosition;
            
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

    private void StartBash()
    {
        if(bashPhase != BashPhase.Start)
        {
            bashPhase = BashPhase.Start;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void DuringBash()
    {
        if(bashPhase == BashPhase.Active)
        {

        }
    }

    private void EndBash()
    {
        bashPhase = BashPhase.None;
        Time.timeScale = 1;
    }

}

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
    public GameObject bashArrowPrefab;
    private GameObject bashArrow;

    private float move;
    private float bash;
    private float jump;

    private Vector3 mouseDelta, mousePos3D, mousePos;

    private float time = 0;
    private readonly float lerpDuration = 0.5f;


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

    private void Update()
    {
        BashMove();
        Camera.main.transform.position = transform.position + new Vector3(0, 5, -10);

        if (bashPhase == BashPhase.Start || bashPhase == BashPhase.Active)
        {
            if(time < lerpDuration)
            {
                bashPhase = BashPhase.Active;
                Time.timeScale = Mathf.Lerp(1, 0, time/lerpDuration);
                time += Time.deltaTime;
                DuringBash();
            }
            else
            {
                Time.timeScale = 0;
                DuringBash();
            }
        }

        if (bash == 1)
        {
            StartBash();
        }
        else if ((bash == 0 && bashPhase != BashPhase.None) && bashPhase != BashPhase.End)
        {
            bashPhase = BashPhase.End;
            EndBash();
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
            mousePos.z = -Camera.main.transform.position.z;
            mousePos3D = Camera.main.ScreenToWorldPoint(mousePos);
            mouseDelta = mousePos3D - bashObject.transform.position;

            float maxMagnitude = bashObject.GetComponent<SphereCollider>().radius;
            if(mouseDelta.magnitude > maxMagnitude)
            {
                mouseDelta.Normalize();
                mouseDelta *= maxMagnitude;
            }

            bashArrow.transform.position = bashObject.transform.position + mouseDelta;
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

        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y / 2 + 1f))
        {
            grounded = true;
        }

        return grounded;
    }

    private void StartBash()
    {
        if(bashPhase == BashPhase.None)
        {
            bashPhase = BashPhase.Start;
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            move = 0;
            jump = 0;
            bashArrow = Instantiate(bashArrowPrefab, bashObject.transform.position, Quaternion.identity, transform.parent);
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void DuringBash()
    {
        if(bashPhase != BashPhase.Active)
        {
            bashPhase = BashPhase.Active;
        }
        else
        {

        }
    }

    private void EndBash()
    {
        bashObject.Bashed();
        bashPhase = BashPhase.None;
        Time.timeScale = 1;
        GetComponent<Rigidbody>().isKinematic = false;
        Destroy(bashArrow);
        GetComponent<Rigidbody>().velocity = mouseDelta * 7f;
        if (bashObject.isProjectile)
        {
            bashObject.GetComponent<Rigidbody>().velocity = -mouseDelta * 7f;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Author: [Morgan, Chase]
 * Last Updated: [12/07/2023]
 * [Handles everything to do with input for player movement and bashing]
 */

public enum BashPhase
{
    None,
    Start,
    Active,
    End
}

public class PlayerController : MonoBehaviour
{
    //set up a singleton for easier use
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    //Public variables to be set in the inspector and used in other classes
    public float speed = 10f;
    public float jumpHeight = 10f;
    public float bashTime;
    public bool canBash, inRange;
    public BashPhase bashPhase = 0;
    public BashableObject bashObject;
    public GameObject bashArrowPrefab;

    //Private GameObject for the instantiating of bashArrow (just a square currently)
    private GameObject bashArrow;

    //Private variables to handle various things in the program
    private float move, bash, jump, time;
    private Vector3 mouseDelta, mousePos3D, mousePos;
    private readonly float lerpDuration = 0.5f;


    private void Awake()
    {
        //Set up the singleton
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
        //Physics based movement
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
        //Movement not using the physics engine
        BashMove();
        Camera.main.transform.position = transform.position + new Vector3(0, 5, -10);

        //Slowing down time for the bash (not noticeable because there isn't anything to reference in the scene like Ori)
        if (bashPhase == BashPhase.Start || bashPhase == BashPhase.Active)
        {
            if(time < lerpDuration)
            {
                bashPhase = BashPhase.Active;
                Time.timeScale = Mathf.Lerp(1, 0, time/lerpDuration);
                time += Time.deltaTime;
                DuringBash(); //The Bash is now active, so fire the DuringBash() method
            }
            else
            {
                Time.timeScale = 0;
                DuringBash();
            }
        }

        //If bash is being held, start the bash (if not being held, end the bash)
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

    /// <summary>
    /// Moves the player if not bashing
    /// </summary>
    /// <param name="context">the context</param>
    public void Move(InputAction.CallbackContext context)
    {
        if(bashPhase == 0)
        {
            move = context.ReadValue<float>();
        }
    }

    /// <summary>
    /// Starts the bash
    /// </summary>
    /// <param name="context">the context</param>
    public void Bash(InputAction.CallbackContext context)
    {
        if (canBash && inRange && bashObject != null)
        {
            bash = context.ReadValue<float>();
        }
    }

    /// <summary>
    /// Moves the bashArrow around a circle
    /// </summary>
    private void BashMove()
    {
        //Check if active as another layer of protection
        if(bashPhase == BashPhase.Active)
        {
            //Get all mouse information needed
            mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            mousePos3D = Camera.main.ScreenToWorldPoint(mousePos);
            mouseDelta = mousePos3D - bashObject.transform.position;

            //Do a similar thing to Mission Demolition with the slingshot (this one is reversed)
            float maxMagnitude = bashObject.GetComponent<SphereCollider>().radius;
            if(mouseDelta.magnitude > maxMagnitude)
            {
                mouseDelta.Normalize();
                mouseDelta *= maxMagnitude;
            }

            bashArrow.transform.position = bashObject.transform.position + mouseDelta;
        }
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    /// <param name="context">the context</param>
    public void Jump(InputAction.CallbackContext context)
    {
        if(bashPhase == BashPhase.None)
        {
            jump = context.ReadValue<float>();
        }
    }

    /// <summary>
    /// Prevents the player from bashing for amount of time provided
    /// </summary>
    /// <returns>the time to wait</returns>
    private IEnumerator BashCooldown()
    {
        canBash = false;
        yield return new WaitForSeconds(bashTime);
        canBash = true;
    }

    /// <summary>
    /// Checks if the player is on the ground or not using Raycasting
    /// </summary>
    /// <returns>Returns true if grounded</returns>
    private bool isGrounded()
    {
        bool grounded = false; //Assume the player is not grounded

        if (Physics.Raycast(transform.position, Vector3.down, transform.localScale.y / 2 + 1f))
        {
            grounded = true;
        }

        return grounded;
    }
    
    /// <summary>
    /// Starts the bash sequence
    /// </summary>
    private void StartBash()
    {
        //Check if the bashPhase = 0
        if(bashPhase == BashPhase.None)
        {
            //Set up everything that needs to be used for bash and disable movement
            bashPhase = BashPhase.Start;
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            move = 0;
            jump = 0;
            bashArrow = Instantiate(bashArrowPrefab, bashObject.transform.position, Quaternion.identity, transform.parent);
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    /// <summary>
    /// Fires during the bash sequence
    /// </summary>
    private void DuringBash()
    {
        if(bashPhase != BashPhase.Active)
        {
            bashPhase = BashPhase.Active;
        }
    }

    /// <summary>
    /// Ends the bash sequence
    /// </summary>
    private void EndBash()
    {
        //Resets all values and triggers the object bash method
        //Also gives the velocity to the player and projectile based on where the arrow was
        bashObject.Bashed();
        bashPhase = BashPhase.None;
        Time.timeScale = 1;
        GetComponent<Rigidbody>().isKinematic = false;
        Destroy(bashArrow);
        GetComponent<Rigidbody>().velocity = mouseDelta * 7f;
        StartCoroutine(BashCooldown());
        if (bashObject.isProjectile)
        {
            bashObject.GetComponent<Rigidbody>().velocity = -mouseDelta * 7f;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Morgan, Chase]
 * Last Updated: [12/07/2023]
 * [Handles the BashObject that can be interacted by the player]
 */

public class BashableObject : MonoBehaviour
{
    //Public bool variables for other classes to access
    public bool isBashable = true;
    public bool isProjectile = false;

    //Private serialized float to give cooldown time
    [SerializeField]
    private float cooldownTime = 2f;

    //Private color that is used for the intial color of this object
    private Color matColor;

    private void Awake()
    {
        //Set matColor to the starting color
        matColor = GetComponent<MeshRenderer>().material.color;
    }

    /// <summary>
    /// Makes the object not bashable until the provided cooldown time is up
    /// </summary>
    /// <returns>Time to wait</returns>
    private IEnumerator BashCooldown()
    {
        GetComponent<MeshRenderer>().material.color = Color.gray;
        isBashable = false;
        yield return new WaitForSeconds(cooldownTime);
        isBashable = true;
        GetComponent<MeshRenderer>().material.color = matColor;
    }

    /// <summary>
    /// Fires when the object is bashed
    /// </summary>
    public void Bashed()
    {
        StartCoroutine(BashCooldown());
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if the tag is the player and if the object can be bashed currently
        if (other.CompareTag("Player") && isBashable)
        {
            other.transform.GetComponent<PlayerController>().bashObject = this;
            other.transform.GetComponent<PlayerController>().inRange = true;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
        else if((!other.CompareTag("Player") && !other.CompareTag("BashObject")) && isProjectile) //If not the player or another bash object and is a projectile, destroy this object
        {
            //If it hit an enemy, hurt the enemy by 1
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().health--;
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerController>().bashObject = null;
            other.transform.GetComponent<PlayerController>().inRange = false;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}

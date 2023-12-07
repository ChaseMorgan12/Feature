using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (other.CompareTag("Player") && isBashable)
        {
            other.transform.GetComponent<PlayerController>().bashObject = this;
            other.transform.GetComponent<PlayerController>().canBash = true;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
        else if((!other.CompareTag("Player") && !other.CompareTag("BashObject")) && isProjectile)
        {
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
            other.transform.GetComponent<PlayerController>().canBash = false;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}

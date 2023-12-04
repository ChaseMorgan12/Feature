using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashableObject : MonoBehaviour
{
    public bool isBashable = true;

    [SerializeField]
    private float cooldownTime = 2f;

    private void Awake()
    {
        
    }

    private IEnumerator BashCooldown()
    {
        isBashable = false;
        yield return new WaitForSeconds(cooldownTime);
        isBashable = true;
    }

    public void Bashed()
    {
        StartCoroutine(BashCooldown());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().bashObject = this;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().bashObject = null;
        }
    }
}

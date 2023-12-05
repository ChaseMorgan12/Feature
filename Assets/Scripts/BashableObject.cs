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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerController>().bashObject = this;
            other.transform.GetComponent<PlayerController>().canBash = true;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerController>().bashObject = null;
            other.transform.GetComponent<PlayerController>().canBash = false;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
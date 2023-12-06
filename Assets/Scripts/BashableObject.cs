using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashableObject : MonoBehaviour
{
    public bool isBashable = true;
    public bool isProjectile = false;

    [SerializeField]
    private float cooldownTime = 2f;

    private Color matColor;

    private void Awake()
    {
        matColor = GetComponent<MeshRenderer>().material.color;
    }

    private IEnumerator BashCooldown()
    {
        GetComponent<MeshRenderer>().material.color = Color.gray;
        isBashable = false;
        yield return new WaitForSeconds(cooldownTime);
        isBashable = true;
        GetComponent<MeshRenderer>().material.color = matColor;
    }

    public void Bashed()
    {
        StartCoroutine(BashCooldown());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && isBashable)
        {
            other.transform.GetComponent<PlayerController>().bashObject = this;
            other.transform.GetComponent<PlayerController>().canBash = true;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
        else if(!other.transform.CompareTag("Player") && isProjectile)
        {
            Destroy(gameObject);
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

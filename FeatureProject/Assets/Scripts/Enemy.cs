using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Morgan, Chase]
 * Last Updated: [12/07/2023]
 * [Basic enemy class to give projectiles a target to test]
 */
public class Enemy : MonoBehaviour
{
    public int health = 1;

    private void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

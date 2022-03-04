﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : EnemyPathMovementt
{
    //Calls All functions from the enemy Controller
    [SerializeField] float radius;
    [SerializeField] float aggroRadius;
    public Animator racoonAnim;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        //Debug.Log("Passive StartCO");
        //If player is in radius start chase else Patrol
        if (Vector2.Distance(transform.position, player.position) <= radius)
        {
            //Debug.Log("Player in radius");
            isChaseing = true;
            StopCoroutine(Chase());
            StartCoroutine(Chase());
        }

        if (Vector2.Distance(transform.position, player.position) <= aggroRadius)
        {

            racoonAnim.SetBool("isAngry", true);
            //Debug.Log("agrro");
        }
        
        else
        {
            racoonAnim.SetBool("isAngry", false);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(FindPlayer());

    }
}

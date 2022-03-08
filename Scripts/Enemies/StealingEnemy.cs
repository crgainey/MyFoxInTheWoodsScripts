using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingEnemy : EnemyPathMovementt
{
    //Calls All funstions from the enemy Controller
    [SerializeField] float radius;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(StartUpdate());
    }

    IEnumerator StartUpdate()
    {
        //if player approches will go to runaway function
        if (Vector2.Distance(transform.position, player.position) <= radius)
        {
            StartCoroutine(Runaway());
        }

        //if near food will steal/ destroy pickup
        //if (Vector2.Distance(transform.position, foodPickup.transform.position) <= radius)
        //{
        //    StartCoroutine(Steal());
        //}

        yield return new WaitForSeconds(1f);
        StartCoroutine(StartUpdate());
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgressiveEnemeyAI : EnemyPathMovementt
{
    //Calls All funstions from the enemy Controller
    [SerializeField] float radius;

    protected override void Start()
    {
        Restart();
    }

    public void Restart()
    {
        ReStartMain();
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(4f);

        //If player is in radius start chase else Patrol
        if (Vector2.Distance(transform.position, player.position) <= radius)
        {
            isChaseing = true;
            StopCoroutine(Chase());
            StartCoroutine(Chase());
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(FindPlayer());
    }

}

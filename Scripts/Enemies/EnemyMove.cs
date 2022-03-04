using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    //chase 
    public Transform player;
    public float chaseRange;
    public float distanceToTarget;

    public float speed;

    //patrol 
    public Transform[] wayPoints;
    Transform currentWayPoint;
    int currentWayIndex;

    void Start()
    {
        currentWayIndex = 0;
        currentWayPoint = wayPoints[currentWayIndex];
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position, player.position);

        if (distanceToTarget > chaseRange)
        {
            Patrol();
        }

        if (distanceToTarget < chaseRange)
        {
            Chase();
        }
    }

    void Patrol()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
        // reach patrol point?
        if (Vector3.Distance(transform.position, currentWayPoint.position) < .1f)
        {
            // has reached point, check to see if more
            if (currentWayIndex + 1 < wayPoints.Length)
            {
                currentWayIndex++;
            }
            else
            {
                currentWayIndex = 0;
            }

            currentWayPoint = wayPoints[currentWayIndex];
        }

        // face patrol point
        Vector3 wayPatrolDir = currentWayPoint.position - transform.position;
        float angle = Mathf.Atan2(wayPatrolDir.y, wayPatrolDir.x) * Mathf.Rad2Deg - 90f;

        //rotate enemy to face waypoint
        Quaternion enemy = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, enemy, 45f);
    }

    void Chase()
    {
        Vector3 targetDir = player.position - transform.position;
        float angleW = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targ = Quaternion.AngleAxis(angleW, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targ, 180f);

        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }
}
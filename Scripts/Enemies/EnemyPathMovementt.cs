using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the moving object that finds a direct path to player
public class EnemyPathMovementt : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float speed;

    public bool isChaseing = false;
    bool hitPlayer = false;

    Vector2[] path;
    int targetIndex;

    public Transform player;
    public Transform[] waypoints;

    public Animator anim;
    public SpriteRenderer sp;

    public float startWaitTime = 2;
    float _waitTime;
    int randomWaypoint;

    GameObject foodObj;
    CharacterController cc;
    EnemySpawner spawner;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cc = GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterController>();
        spawner = GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        //foodObj = GameObject.FindGameObjectWithTag("Pickup");
    }

    protected virtual void Start()
    {
        ReStartMain();
    }

    public void ReStartMain()
    {
        //Debug.Log("Restartmain called");
        _waitTime = startWaitTime;
        randomWaypoint = Random.Range(0, waypoints.Length);

        GameObject ccObject = GameObject.FindWithTag("Character");
        if (ccObject != null)
        {
            cc = ccObject.GetComponent<CharacterController>();
        }
        if (ccObject == null)
        {
            Debug.Log("Cannot find 'CC script");
        }
    }

    void Update()
    {
        //StartCoroutine(StartUpdate());
        if (Vector2.Distance(transform.position, player.position) <= 50)
        {
            //stops patrol if true
            if (isChaseing == false)
            {
                anim.SetBool("isRunning", true);
                transform.position = Vector2.MoveTowards(transform.position, waypoints[randomWaypoint].position, speed * Time.deltaTime);
                sp.flipX = waypoints[randomWaypoint].position.x > transform.position.x;
                if (Vector2.Distance(transform.position, waypoints[randomWaypoint].position) < 0.2)
                {
                    anim.SetBool("isRunning", false);
                    Patrol();
                }
            }
        }
    }

    public void Patrol()
    {
        if (_waitTime <= 0)
        {
            randomWaypoint = Random.Range(0, waypoints.Length);
            _waitTime = startWaitTime;
        }
        else
        {
            _waitTime -= Time.deltaTime;
        }
    }

    //public IEnumerator Patrol()
    //{
    //    //Debug.Log("Patrol");

    //    if (_waitTime <= 0)
    //    {
    //        anim.SetBool("isRunning", false);
    //        randomWaypoint = Random.Range(0, waypoints.Length);
    //        _waitTime = startWaitTime;
    //    }
    //    else
    //    {
    //        _waitTime -= Time.deltaTime;
    //    }

    //    path = Pathfinding.RequestPath(transform.position, waypoints[randomWaypoint].position);
    //    StartCoroutine("FollowPath");
    //    StopCoroutine("FollowPath");

    //    yield return new WaitForSeconds(.25f);
    //}

    //if we hit end of path the refresh and call FollowPath again
    public IEnumerator Chase()
    {
        yield return new WaitForSeconds(.25f);
        Vector2 targetPosOld = (Vector2)player.position + Vector2.up;

        while (true)
        {
            if (targetPosOld != (Vector2)player.position)
            {
                targetPosOld = (Vector2)player.position;

                path = Pathfinding.RequestPath(transform.position, player.position);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
                anim.SetBool("isRunning", true);
            }
            //Debug.Log("CHASING");
            yield return new WaitForSeconds(1f);
        }
        
    }

    public IEnumerator Runaway()
    {
        Vector2 targetPosOld = (Vector2)player.position + Vector2.up;

        while (true)
        {
            if (targetPosOld != (Vector2)player.position)
            {
                targetPosOld = (Vector2)player.position;
                path = Pathfinding.RequestPath(transform.position, -player.position);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");

                anim.SetBool("isRunning", true);
                //Debug.Log("RUNAWAY");
                isChaseing = true;
            }
            yield return new WaitForSeconds(10f);
            spawner.DoTHING();
            Destroy(gameObject);
        }
        
    }
    //public IEnumerator Steal()
    //{

    //    GameObject foodPickup = GameObject.FindGameObjectWithTag("Pickup");
    //    Vector2 targetPosOld = (Vector2)player.position + Vector2.up;

    //    while (true)
    //    {
    //        if (targetPosOld != (Vector2)player.position)
    //        {
    //            targetPosOld = (Vector2)player.position;
    //            path = Pathfinding.RequestPath(transform.position, foodPickup.transform.position);
    //            StopCoroutine("FollowPath");
    //            StartCoroutine("FollowPath");


    //            anim.SetBool("isRunning", true);
    //            //Destroy(foodObj);
    //            // Debug.Log("STEAL");
    //        }
    //        yield return new WaitForSeconds(.25f);
    //    }
        
    //}
   

    //starts us following the path
    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            targetIndex = 0;
            Vector2 currentWaypoint = path[0];

            while (true)
            {
                if ((Vector2)transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                sp.flipX = player.transform.position.x > transform.position.x;
                anim.SetBool("isRunning", true);
                yield return null;
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Enemy Did Damage");
            other.GetComponent<Player>().TakeDamage(damage);
            
        }

        if (other.CompareTag("Character") && cc.isPressed == true)
        {
            anim.SetBool("isHit", true);
            StartCoroutine(EndAnimation());
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitForDamage());
            other.GetComponent<Player>().TakeDamage(0.2f);
        }
    }

    IEnumerator WaitForDamage()
    {
        yield return new WaitForSeconds(2);
    }
    IEnumerator EndAnimation()
    {
        //Debug.Log("Ending Anim");
        yield return new WaitForSeconds(1f);
        //GameObject.FindGameObjectWithTag("EnemyAnim").GetComponent<Animator>().SetBool("isHit", false);
        anim.SetBool("isHit", false);
    }

    //Visual
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}



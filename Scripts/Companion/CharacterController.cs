using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] float smoothSpeed = 0.5f;
    [SerializeField] float dragSpeed = 6;
    [SerializeField] float waitTime = 1;
    [SerializeField] float maxDragDistance = 8f;
    [SerializeField] public float damage = 2f;

    Vector3 offset = new Vector3(0,0,0);
    Vector3 startPos;
    Vector3 endPos;

    public bool isPressed = false;
    public bool isDrag = false;

    public GameObject featherDrop;
    public AudioSource collectAudio;

    public Animator anim;
    public Rigidbody2D rb;
    public LineRenderer lr;
    public SpriteRenderer sprite;
    Transform enemy;
    Transform player;
    Camera cam;
    Rigidbody2D anchor;

    Player playerScr;
    PlayerMovement playerScript;

    void Start()
    {
        //enemyAnim = GameObject.FindGameObjectWithTag("EnemyAnim").GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().transform;
        playerScr = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anchor = GameObject.FindGameObjectWithTag("Anchor").GetComponent<Rigidbody2D>();

        cam = Camera.main;

        lr.enabled = false;
        //lr.useWorldSpace = true;

        GameObject playerMove = GameObject.FindWithTag("Player");
        if (playerMove != null)
        {
            playerScript = playerMove.GetComponent<PlayerMovement>();
        }
        if (playerMove == null)
        {
            Debug.Log("Cannot find 'Player Script");
        }

    }

    void Update()
    {
        //Has the bird follow the player
        if (isPressed == false)
        {
            if(playerScript.faceRight == true)
            {
                offset = new Vector3(-2, 1, 0);
                Vector3 desiredPosition = player.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
                sprite.flipX = player.transform.position.x > transform.position.x;
            }
            else
            {
                
                offset = new Vector3(4, 1, 0);
                Vector3 desiredPosition = player.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
                sprite.flipX = player.transform.position.x > transform.position.x;
            }
                
        }

        //Checks for drag
        if(isDrag == true)
        {
            OnDrag();
        }
        if (playerScr.defense <= 0) 
        {
            StartCoroutine(EndAttackBouns());
        }

    }

    void OnMouseDown()
    {
        startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        isDrag = true;
        lr.enabled = true;
        anim.SetBool("isDragging", true);
    }

    void OnMouseUp()
    {
        endPos = cam.ScreenToWorldPoint(Input.mousePosition);
        anim.SetBool("isDragging", false);
        rb.AddForce((startPos - endPos) * Vector3.Distance(transform.position, startPos) * dragSpeed);
        isPressed = true;
        sprite.sortingOrder = 5;
        isDrag = false;
        lr.enabled = false;
        StartCoroutine(ReturnToPlayer());
    }

    void OnDrag()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        float distance = Vector3.Distance(mousePos, anchor.position);
        
        if (distance >= maxDragDistance)
        {
            Vector2 fromOrgin = mousePos - anchor.position; //ORGIN OF VECTOR2
            fromOrgin *= maxDragDistance / distance;
            mousePos = anchor.position + fromOrgin;
            transform.position = mousePos;
        }
        else
        {
            transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }

        Vector3 inverseMousePos = new Vector3(Screen.width - Input.mousePosition.x, Screen.height - Input.mousePosition.y, Input.mousePosition.z);
        var lineMousePos = cam.ScreenToWorldPoint(inverseMousePos);
        //Vector3 lineopp = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x +1, Input.mousePosition.y +1, Input.mousePosition.z));
        //Debug.DrawLine(rb.position, lineMousePos, Color.white, 2.5f);

        Vector3[] positions = new Vector3[3];
        positions[0] = rb.position;
        positions[1] = lineMousePos;
        lr.SetPositions(positions);

    }


    //so the bird isn't flying in space forever
    IEnumerator ReturnToPlayer()
    {
        yield return new WaitForSeconds(waitTime);
        rb.velocity = Vector3.zero; //if we dont reset it keeps building up 
        isPressed = false;
        sprite.sortingOrder = 3;
        anim.SetBool("isDragging", false);
        anim.SetBool("isHit", false);
    }

    IEnumerator EndAttackBouns()
    {
        yield return new WaitForSeconds(10f);
        damage = 2f;
    }

    //THIS IS GARBAGE NEEDS TO BE FIXED
    void AutoAttack()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        transform.position = enemy.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if(playerScript.playerHit == true)
        //{
        //    if (other.CompareTag("Enemy"))
        //    {
        //        //Debug.Log("Bird Enemy Hit");
        //        anim.SetBool("isHit", true);
        //        GameObject.FindGameObjectWithTag("EnemyAnim").GetComponent<Animator>().SetBool("isHit", true);
        //        StartCoroutine(EndAnimation());
        //        other.GetComponent<EnemyHealth>().TakeDamage(damage);
        //        //AutoAttack();
        //    }
        //}

        if (isPressed == true)
        {
            if (other.CompareTag("Enemy"))
            {
                //Debug.Log("Bird Enemy Hit");
                anim.SetBool("isHit", true);
                other.GetComponent<EnemyHealth>().TakeDamage(2f);
                Instantiate(featherDrop, transform.position, transform.rotation);
                StartCoroutine(EndAnimation());
            }

            else if (other.CompareTag("Pickup") || other.CompareTag("TreeItem"))
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.AddItem(Instantiate(other.GetComponent<PickupItem>().item));
                Destroy(other.gameObject);
                collectAudio.Play();
                //Debug.Log("Bird Pickup");
            }
        }
    }

    IEnumerator EndAnimation()
    {
        //Debug.Log("Ending Anim");
        yield return new WaitForSeconds(1f);
        //GameObject.FindGameObjectWithTag("EnemyAnim").GetComponent<Animator>().SetBool("isHit", false);
        anim.SetBool("isHit", false);
    }

}

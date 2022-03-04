using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10;

    Vector3 _targetPos;
    bool isMoving = false;

    CharacterController _cc;
    public Animator anim;
    public Camera cam;
    public bool playerHit = false;
    public bool faceRight = true;

    void Start()
    {
        GameObject ccObject = GameObject.FindWithTag("Character");
        if (ccObject != null)
        {
            _cc = ccObject.GetComponent<CharacterController>();
        }
        if (ccObject == null)
        {
            Debug.Log("Cannot find 'CC script");
        }

    }

    void Update()
    {
        //This is to prevent the fox from moving when trying to sling the bird
        if (Input.GetMouseButton(0) && _cc.isDrag == false)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Clicked UI");
            }

            else

                SetTargetPosition();

        }

        if (isMoving)
        {
            Move();
        }
    }

    //Sets the postion based on the camera
    void SetTargetPosition()
    {
        _targetPos = cam.ScreenToWorldPoint(Input.mousePosition);
        _targetPos.z = transform.position.z;

        isMoving = true;
        anim.SetBool("isRunning",true);
        float posX = Input.mousePosition.x;
        float halfOfScreen = Screen.width / 2;
        //So we can determine which was the player is facing
        if (posX > halfOfScreen)
        {
            //Debug.Log("Right");
            anim.SetBool("isRunningRight", true);
            anim.SetBool("isRunningLeft", false);
            faceRight = true;
        }
        else
        {
            //Debug.Log("Left");
            anim.SetBool("isRunningLeft", true);
            anim.SetBool("isRunningRight", false);
            faceRight = false;
        }

    }

    //moves the player
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, moveSpeed * Time.deltaTime);

        if (transform.position == _targetPos)
        {
            isMoving = false;
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunningRight", false);
            anim.SetBool("isRunningLeft", false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerHit = true;
            anim.SetBool("isHit", true);
            StartCoroutine(EndAnimation());
        }

    }

    IEnumerator EndAnimation()
    {
        //Debug.Log("Ending Anim");
        yield return new WaitForSeconds(1f);
        anim.SetBool("isHit", false);

    }

}
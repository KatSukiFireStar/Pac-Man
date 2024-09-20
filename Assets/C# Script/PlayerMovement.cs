using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;

    private Vector2 movement;
    private Vector2 previousMovement = new(1, 0);
    private Rigidbody2D rb;

    public int speed = 1;
    public float moveTime = 0.2f;
    public Collider2D wall;

    private bool isMoving = false;
    private Vector2 startPos, endPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnMovement(InputValue value)
    {
        Vector2 tmp = value.Get<Vector2>();
        if (tmp.x != 0 && tmp.y != 0)
        {
            if (movement.x != 0)
            {
                tmp.x = 0;
                if (tmp.y < 0)
                {
                    tmp.y = -1;
                } else
                {
                    tmp.y = 1;
                }
            } else if (movement.y != 0)
            {
                tmp.y = 0;
                if (tmp.x < 0)
                {
                    tmp.x = -1;
                } else
                {
                    tmp.x = 1;
                }
            }
        }
        if (tmp.x != 0 || tmp.y != 0)
        {
            Rigidbody2D rb2 = rb;
            rb2.velocity = tmp * speed;
            if (!rb2.IsTouching(wall))
            {
                previousMovement = tmp;
            }
        }
        movement = tmp;
    }

    IEnumerator MovePlayer(Vector2 dir)
    {
        isMoving = true;
        float next = 0f;
        startPos = transform.position;
        endPos = startPos + dir;

        while (next < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, next / moveTime);
            next += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        isMoving = false;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (movement.x < 0)
        {
            animator.SetBool("up", false);
            animator.SetBool("right", false);
            animator.SetBool("down", false);
            animator.SetBool("left", true);
        } else if (movement.x > 0)
        {
            animator.SetBool("up", false);
            animator.SetBool("right", true);
            animator.SetBool("down", false);
            animator.SetBool("left", false);
        } else if (movement.y > 0)
        {
            animator.SetBool("up", true);
            animator.SetBool("right", false);
            animator.SetBool("down", false);
            animator.SetBool("left", false);
        } else if (movement.y < 0)
        {
            animator.SetBool("up", false);
            animator.SetBool("right", false);
            animator.SetBool("down", true);
            animator.SetBool("left", false);
        }
        Rigidbody2D rb2 = rb;
        if (movement.x != 0 || movement.y != 0)
        {
            rb2.velocity = movement * speed;
            if (rb2.IsTouching(wall))
            {
                movement = new();
            }
        } else
        {
            //rb2.velocity = previousMovement * speed;
            //if (!rb2.IsTouching(wall))
            //{
            //    movement = previousMovement;
            //}
        }

        Debug.Log(movement);

        if (!isMoving)
        {
            StartCoroutine(MovePlayer(new(x,y)));
        }
    }
}

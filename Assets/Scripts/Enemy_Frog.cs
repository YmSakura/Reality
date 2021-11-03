using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Frog : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D coll;

    public LayerMask ground;
    public Transform leftpoint, rightpoint;
    public float speed, jumpSpeed;
    private bool faceLeft = true;
    private float leftx, rightx;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        transform.DetachChildren();
        leftx = leftpoint.position.x;
        rightx = rightpoint.position.x;
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimator();
    }

    void SwitchAnimator()
    {
        if (animator.GetBool("jumping"))
        {
            if(rb.velocity.y < 0.1f)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        if (coll.IsTouchingLayers(ground) && animator.GetBool("falling") )
        {
            animator.SetBool("falling", false);
        }
    }

    void Movement()
    {
        if(faceLeft)
        {
            if(coll.IsTouchingLayers(ground))
            {
                animator.SetBool("jumping", true);
                rb.velocity = new Vector2(-speed, jumpSpeed);
            }
                
            if(transform.position.x < leftx)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                faceLeft = false;
            }
        }
        else
        {
            if (coll.IsTouchingLayers(ground))
            {
                animator.SetBool("jumping", true);
                rb.velocity = new Vector2(speed, jumpSpeed);
            }

            if (transform.position.x > rightx)
            {
                transform.localScale = new Vector3(1, 1, 1);
                faceLeft = true;
            }
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    public void JumpOn()
    {
        animator.SetTrigger("death");
    }
}

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Enemy_Eagle : Enemy
{
    //private Rigidbody2D rb;
    public AIPath aiPath;
    

    //public Transform top, bottom;
    //public float speed;

    //private float topY, bottomY;
    //private bool moveUp = true;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //rb = GetComponent<Rigidbody2D>();

        //transform.DetachChildren();
        //topY = top.position.y;
        //bottomY = bottom.position.y;
        //Destroy(top.gameObject);
        //Destroy(bottom.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
        FaceDirection();
    }

    void FaceDirection()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    //void Movement()
    //{
    //    if(moveUp)
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, speed);
    //        if (transform.position.y > topY)
    //            moveUp = false;
    //    }
    //    else
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, -speed);
    //        if (transform.position.y < bottomY)
    //            moveUp = true;
    //    }
    //}
}

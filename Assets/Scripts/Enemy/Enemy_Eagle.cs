using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Eagle : Enemy
{
    //private Rigidbody2D rb;
    

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

    protected override void Death()
    {
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
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

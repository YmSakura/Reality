using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Oppossum : Enemy
{
    public Transform player;
    private Rigidbody2D rb;

    private float moveSpeed = 5f;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }

    void ChasePlayer()
    {
        float distance = transform.position.x - player.position.x;
        if (distance < 10f)
        {
            rb.velocity = Vector2.left * moveSpeed;
        }
    }
    
}

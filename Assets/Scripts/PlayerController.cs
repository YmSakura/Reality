using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public AudioSource jumpAudio, collectionAudio, hurtAudio;
    public Collider2D coll;
    public LayerMask ground;
    public Text cherryNumber;

    public float speed = 400.0f;
    public float jumpForce = 400.0f;
    public int score;
    //默认为false
    private bool isHurt;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHurt)
        {
            Movement();
        }
        SwitchAnimation();
    }

    void Movement()
    {
        //GetAxis ：-1 -> 0 ->1 平滑过度
        float horizontalAxis = Input.GetAxis("Horizontal");
        //GetAxisRaw ：直接返回-1 0 1，没有中间值
        float faceDirection = Input.GetAxisRaw("Horizontal");

        //Time.deltaTime放在Update和FixedUpdate里会自动调整
        //Time.fixedDeltaTime无论放在哪里都是固定的0.02
        //Debug.Log(Time.fixedDeltaTime);

        //角色移动
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //控制运动动画
        animator.SetFloat("running", Mathf.Abs(faceDirection));

        //人物转向
        if (faceDirection != 0)
        {
            transform.localScale = new Vector3(faceDirection, 1, 1);
        }

        //角色跳跃
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            //通过修改速度来实现跳跃，x不变，y变大
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            animator.SetBool("jumping", true);
        }
    }

    //切换动画效果
    void SwitchAnimation()
    {
        if(rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", true);
        }
        
        //正在跳跃
        if (animator.GetBool("jumping"))
        {
            Debug.Log(rb.velocity.y);
            //y轴速度消失，开始下落
            if(rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }else if (isHurt)
        {
            animator.SetBool("hurting", true);
            if (Mathf.Abs(rb.velocity.x) < 1.0f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", false);
        }

    }

    //收集物品
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collection")
        {
            collectionAudio.Play();
            Destroy(collision.gameObject);
            score++;
            cherryNumber.text = score.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //碰到敌人时
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 5);
            }
            else if(transform.position.x < collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(-10, rb.velocity.y);
            }
            else if(transform.position.x > collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(10, rb.velocity.y);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public AudioSource jumpAudio, collectionAudio, hurtAudio;
    public Collider2D coll;
    public Collider2D discoll;
    public LayerMask ground;
    public Text cherryNumber;
    public Transform ceilingCheck, floorCheck;

    public float speed = 400.0f;
    public float jumpForce = 400.0f;
    public int score;

    //是否为受伤状态
    private bool isHurt = false;
    //下蹲检测
    private bool checkHead = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Crouch();
        SwitchAnimation();
        if (!isHurt) Movement();
    }

    void Movement()
    {
        //GetAxis ：-1 -> 0 ->1 平滑过度
        float horizontalAxis = Input.GetAxis("Horizontal");
        //GetAxisRaw ：直接返回-1 0 1，没有中间值
        float faceDirection = Input.GetAxisRaw("Horizontal");

        //角色移动
        rb.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb.velocity.y);
        //running参数的关联
        animator.SetFloat("running", Mathf.Abs(faceDirection));

        //角色转向
        if (faceDirection != 0)
            transform.localScale = new Vector3(faceDirection, 1, 1);

    }

    //切换动画效果
    void SwitchAnimation()
    {
        if (rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", true);
        }

        //正在跳跃
        if (isHurt)
        {
            animator.SetBool("hurting", true);
            if (Mathf.Abs(rb.velocity.x) < 3.0f)
            {
                animator.SetBool("hurting", false);
                isHurt = false;
            }
        }
        else if (animator.GetBool("jumping"))
        {
            //Debug.Log(rb.velocity.y);
            //y轴速度消失，开始下落
            if (rb.velocity.y <= 0)
            {
                animator.SetBool("jumping", false);
                animator.SetBool("falling", true);
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            animator.SetBool("falling", false);
        }

    }

    //触发器检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //吃到樱桃
        if (collision.tag == "Collection")
        {
            collectionAudio.Play();
            collision.GetComponent<Animator>().Play("isGot");
        }
        //碰到DeadLine
        if (collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            //延时2s
            Invoke("Restart", 2f);
        }
    }

    //消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, 10);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(-5, rb.velocity.y);
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                isHurt = true;
                hurtAudio.Play();
                rb.velocity = new Vector2(5, rb.velocity.y);
            }
        }
    }

    //下蹲
    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            animator.SetBool("crouching", true);
            //关闭box collider
            discoll.enabled = false;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            //当下蹲键松下时检测启动
            checkHead = true;
        }
        //检测头顶是否有ground
        if (checkHead)
        {
            //overlap是重叠的意思，函数的作用是检查是否有碰撞体进入了指定的圆形区域
            //当角色头顶没有ground的碰撞体时才可以恢复idle状态
            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, ground))
            {
                animator.SetBool("crouching", false);
                discoll.enabled = true;
                //关闭检测
                checkHead = false;
            }
        }
    }

    //重新加载场景
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //角色跳跃
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && onGround() && !isHurt)
        {
            //通过修改速度来实现跳跃，x不变，y变大
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            animator.SetBool("jumping", true);
        }
    }

    private bool onGround()
    {
        if (Physics2D.OverlapCircle(floorCheck.position, 0.2f, ground))
            return true;
        else
            return false;
    }

    public void CherryCount()
    {
        score++;
        cherryNumber.text = score.ToString();
    }

}
